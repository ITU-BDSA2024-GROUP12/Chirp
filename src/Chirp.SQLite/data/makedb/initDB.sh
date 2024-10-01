#!/usr/bin/env bash
# Get the directory where the script is located
#THIS LINE IS GENERATED WITH CHATGPT. (SIDENOTE; I DONT REALLY KNOW BASH NOR WHY EXACTLY IT WORKS... I AM SORRY).
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

sqlite3 "$1" < "$SCRIPT_DIR/schema.sql"
sqlite3 "$1" < "$SCRIPT_DIR/dump.sql"
