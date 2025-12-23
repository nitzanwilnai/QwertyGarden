#!/bin/bash

# Usage: ./filter_alpha_words.sh input.txt output.txt

INPUT="$1"
OUTPUT="$2"

if [ -z "$INPUT" ] || [ -z "$OUTPUT" ]; then
    echo "Usage: $0 <input.txt> <output.txt>"
    exit 1
fi

# 1. Keep only words with letters A–Z or a–z
# 2. Convert valid words to uppercase
grep -E '^[A-Za-z]+$' "$INPUT" | tr '[:lower:]' '[:upper:]' > "$OUTPUT"

echo "Filtered + uppercased words written to $OUTPUT"

