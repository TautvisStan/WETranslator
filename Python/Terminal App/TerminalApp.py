import argparse
import os


def create_parser():
    parser = argparse.ArgumentParser(description="ML translator app")
    parser.add_argument("-l", "--language", type=str, help="Target language")
    parser.add_argument("-t", "--text", type=str, help="Text")
    return parser

def translate(lang : str, sentence : str):
    with open("test.txt", "a") as f:
        f.write(f"lang: {lang}; text: {sentence}")
    return "TEST"

def main():
    parser = create_parser()
    args = parser.parse_args()
    if args.language and args.text:
        print(translate(args.language, args.text))
    else:
        parser.print_help()

if __name__ == "__main__":
    main()