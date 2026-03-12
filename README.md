# C# JSON reader

JSON reader console app

---

## App functions

- working with text files;
- full error validation;
- logging in a .txt file;
- console output.
---

## Supported Actions

- DELETE;
- CREATE;
- UPPERCASE;
- LOWERCASE;
- REMOVEDUPS;
- COPY;
- MOVE;
- READ;
- REPLACE.
---

## JSON format example

```
{
  "files": [
    {
      "path": "input.txt",
      "action": "CREATE",
      "content": "Hello World"
    },
    {
      "path": "source.txt",
      "action": "COPY",
      "destinationPath": "backup/source.txt"
    },
    {
      "path": "data.txt",
      "action": "REPLACE"
    }
  ],
  "delay": 1000
}
