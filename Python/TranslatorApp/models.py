# import keras
# import keras_hub
# from keras import ops

import tensorflow as tf
from keras.models import load_model
from keras_layers import TransformerDecoder, TransformerEncoder, PositionalEmbedding
from keras import ops
import numpy as np
from keras.layers import TextVectorization
import tensorflow.strings as tf_strings
import string
import re
    
strip_chars = string.punctuation + "¿"
strip_chars = strip_chars.replace("[", "")
strip_chars = strip_chars.replace("]", "")

def custom_standardization(input_string):
    lowercase = tf_strings.lower(input_string)
    return tf_strings.regex_replace(lowercase, "[%s]" % re.escape(strip_chars), "")

class ModelKeras():
    def __init__(self, src_vec, src_vocab, tgt_vec, tgt_vocab, model, max_length):

        self.transformer = load_model(model, custom_objects={
            "TransformerEncoder": TransformerEncoder,
            "TransformerDecoder": TransformerDecoder,
            "PositionalEmbedding": PositionalEmbedding
        })

        # Load the source vectorizer
        self.loaded_src_config = np.load(src_vec, allow_pickle=True).item()
        self.loaded_src_vocab = np.load(src_vocab, allow_pickle=True)
        self.loaded_src_vectorization = TextVectorization(**self.loaded_src_config)
        self.loaded_src_vectorization.set_vocabulary(self.loaded_src_vocab)

        # Load the target vectorizer
        self.loaded_tgt_config = np.load(tgt_vec, allow_pickle=True).item()
        self.loaded_tgt_vocab = np.load(tgt_vocab, allow_pickle=True)
        self.loaded_tgt_vectorization = TextVectorization(**self.loaded_tgt_config)
        self.loaded_tgt_vectorization.set_vocabulary(self.loaded_tgt_vocab)


        # self.tgt_vocab = self.loaded_tgt_vectorization.get_vocabulary()
        self.tgt_index_lookup = dict(zip(range(len(self.loaded_tgt_vocab)), self.loaded_tgt_vocab))
        self.max_decoded_sentence_length = max_length


    def translate(self, input_sentence):
        tokenized_input_sentence = self.loaded_src_vectorization([input_sentence])
        decoded_sentence = "[start]"
        for i in range(self.max_decoded_sentence_length):
            tokenized_target_sentence = self.loaded_tgt_vectorization([decoded_sentence])[:, :-1]
            predictions = self.transformer(
                {
                    "encoder_inputs": tokenized_input_sentence,
                    "decoder_inputs": tokenized_target_sentence,
                }
            )

            sampled_token_index = ops.convert_to_numpy(
                ops.argmax(predictions[0, i, :])
            ).item(0)
            sampled_token = self.tgt_index_lookup[sampled_token_index]
            decoded_sentence += " " + sampled_token

            if sampled_token == "[end]":
                break
        decoded_sentence = decoded_sentence.replace("[PAD]", "").replace("[start]", "").replace("[end]", "").strip()
        return decoded_sentence
    

    
# NEVEIKIA NES "ImportError: WordPieceTokenizer requires `tensorflow` and `tensorflow-text` for text processing. 
# Run `pip install tensorflow-text` to install both packages or visit https://www.tensorflow.org/install"
# BET ! ! ! ! !
# ERROR: Could not find a version that satisfies the requirement tensorflow-text (from versions: none)
# ERROR: No matching distribution found for tensorflow-text
# class KerasHubModel(Model):
#     def __init__(self, src_vocab, tgt_vocab, model):
#         self.loaded_model = keras.models.load_model(model)
#         with open(src_vocab, "r", encoding="UTF-8") as f:
#             self.loaded_src_vocab = [line.strip() for line in f]

#         with open(tgt_vocab, "r", encoding="UTF-8") as f:
#             self.loaded_tgt_vocab = [line.strip() for line in f]

#         self.loaded_src_tokenizer = keras_hub.tokenizers.WordPieceTokenizer(
#             vocabulary=self.loaded_src_vocab, lowercase=False
#         )
#         self.loaded_tgt_tokenizer = keras_hub.tokenizers.WordPieceTokenizer(
#             vocabulary=self.loaded_tgt_vocab, lowercase=False
#         )

#     def Translate(self, input_sentences, max_seq_length):
#         batch_size = 1

#         encoder_input_tokens = ops.convert_to_tensor(self.loaded_src_tokenizer(input_sentences))
#         encoder_input_tokens = encoder_input_tokens[:, :max_seq_length]

#         def next(prompt, cache, index):
#             logits = self.loaded_model([encoder_input_tokens, prompt])[:, index - 1, :]
#             hidden_states = None
#             return logits, hidden_states, cache

#         length = max_seq_length + 1
#         start = ops.full((batch_size, 1), self.loaded_tgt_tokenizer.token_to_id("[START]"))
#         pad = ops.full((batch_size, length - 1), self.loaded_tgt_tokenizer.token_to_id("[PAD]"))
#         prompt = ops.concatenate((start, pad), axis=-1)

#         generated_tokens = keras_hub.samplers.GreedySampler()(
#             next,
#             prompt,
#             stop_token_ids=[self.loaded_tgt_tokenizer.token_to_id("[END]")],
#             index=1,
#         )
#         generated_sentences = self.loaded_tgt_tokenizer.detokenize(generated_tokens)
#         return generated_sentences

# from transformers import AutoModel
# class HuggingFaceModel(Model):
#     def __init__(self):

#         self.model = AutoModel.from_pretrained('./HuggingFace/en-lt-t5-small')

#     def Translate(self, input_sentence):
#         translated = self.model.generate(**self.tokenizer(input_sentence, return_tensors="pt", padding=True))
        
#         print(self.tokenizer.decode(translated, skip_special_tokens=True))