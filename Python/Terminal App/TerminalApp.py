import sys
import os
# def create_parser():
#     parser = argparse.ArgumentParser(description="ML translator app")
#     parser.add_argument("-l", "--language", type=str, help="Target language")
#     parser.add_argument("-t", "--text", type=str, help="Text")
#     return parser

def process_single_input(input_line):
    # Parse the input (could be JSON, a specific format, etc.)
    parts = input_line.strip().split('|')  # Using | as delimiter example
    lang = parts[0]
    text = parts[1]
    
    # Process the input
    result = translate_text(lang, text)
    
    # Write result to stdout
    print(result, flush=True)
    # Optional: write errors to stderr
    # sys.stderr.write("Error message\n")
    # sys.stderr.flush()

def translate_text(lang, text):
    # Your translation logic here
    return f"Translated '{text}' to {lang}"

def main():
    # # Check if args were provided (for compatibility with old usage)
    # if len(sys.argv) > 1:
    #     parser = create_parser()
    #     args = parser.parse_args()
    #     result = translate_text(args.language, args.text)
    #     print(result)
    #     return
    with open("test.txt", "a") as f:
        f.write(f"2READY \n")
    # Interactive mode for process pooling
    try:
        # Optionally write ready signal
        # print("READY", flush=True)

        # Process input lines indefinitely
        for line in sys.stdin:
            if line.strip().lower() == "exit":
                break
            process_single_input(line)
    except Exception as e:
        sys.stderr.write(f"Error: {str(e)}\n")
        sys.stderr.flush()

if __name__ == "__main__":
    main()

