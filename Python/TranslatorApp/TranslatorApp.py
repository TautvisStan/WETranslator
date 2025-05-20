import sys
import os
import io
from models import ModelKeras, custom_standardization



sys.stdin = io.TextIOWrapper(sys.stdin.buffer, encoding='utf-8')
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

def process_single_input(input_line):
    parts = input_line.strip().split('|')
    lang = parts[0]
    text = parts[1]
    result = translate_text(lang, text)
    print(result, flush=True)

test = ModelKeras("eng_vectorization.keras", "spa_vectorization.keras", "transformer_translation_model.keras")
def translate_text(lang, text):
    text = test.translate(text)
    return f"{text}"

def main():
    # print(translate_text("", "Hello Code Academy"))
    try:
        for line in sys.stdin:
            if line.strip().lower() == "exit":
                break
            process_single_input(line)
    except Exception as e:
            sys.stderr.write(f"Error: {str(e)}\n")
            sys.stderr.flush()

if __name__ == "__main__":
    main()

