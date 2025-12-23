#!/bin/bash

# Usage: ./clean_words.sh input.txt output.txt

INPUT="$1"
OUTPUT="$2"

if [ -z "$INPUT" ] || [ -z "$OUTPUT" ]; then
    echo "Usage: $0 <input.txt> <output.txt>"
    exit 1
fi

# Clean the input:
# 1. Remove non-alpha chars
# 2. Convert to uppercase
# 3. Remove empty lines
# 4. Sort and remove duplicates

cleaned=$(sed 's/[^A-Za-z]//g' "$INPUT" \
    | tr '[:lower:]' '[:upper:]' \
    | sed '/^$/d' \
    | sort -u)

# Write the cleaned list to output
echo "$cleaned" > "$OUTPUT"

# Count the total cleaned unique words
count=$(echo "$cleaned" | wc -l | tr -d ' ')

echo "Cleaned words written to $OUTPUT"
echo "Total unique words: $count"

