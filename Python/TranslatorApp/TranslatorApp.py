import sys
import os
import io
from models import ModelKeras, custom_standardization

sys.stdin = io.TextIOWrapper(sys.stdin.buffer, encoding='utf-8')
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

def process_single_input(input_line, models):
    input_line = input_line.strip().replace('\ufeff', '')
    parts = input_line.strip().split('|')
    lang = parts[0]
    text = parts[1]
    result = translate_text(lang, text, models)
    print(result, flush=True)

def translate_text(lang, text, models):
    model = models[lang]
    text = model.translate(text)
    return f"{text}"

def main():
    try:
        path = r"BepInEx/plugins/Translator/"
        models = {}
        models["LT"] = ModelKeras(path +"eng_vectorization_config_EnLt.npy", 
                                path +"eng_vocab_EnLt.npy",
                                path +"ltu_vectorization_config_EnLt.npy", 
                                path +"ltu_vocab_EnLt.npy",
                                path +"transformer_translation_model_EnLt.keras",
                                20)
        
        models["ES"] = ModelKeras(path +"eng_vectorization_config_EnEs.npy", 
                                path +"eng_vocab_EnEs.npy",
                                path +"spa_vectorization_config_EnEs.npy", 
                                path +"spa_vocab_EnEs.npy",
                                path +"transformer_translation_model_EnEs.keras",
                                20)
        for line in sys.stdin:
            if line.strip().lower() == "exit":
                break
            process_single_input(line, models)
    except Exception as e:
            sys.stderr.write(f"Error: {str(e)}\n")
            sys.stderr.flush()

if __name__ == "__main__":
    main()

