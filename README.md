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
      "path": "Tests/Old/to_delete.txt",
      "action": "CREATE",
      "content": "Этот файл будет удалён."
    },
    {
      "path": "Tests/Old/to_delete.txt",
      "action": "DELETE",
      "content": null
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "CREATE",
      "content": "Привет, мир! Это тестовый файл. Привет ещё раз."
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "UPPERCASE",
      "content": null
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "LOWERCASE",
      "content": null
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "REMOVEDUPS",
      "content": null
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "COPY",
      "destinationPath": "Tests/New/sample.txt"
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "MOVE",
      "destinationPath": "Tests/New/sample.txt"
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "READ",
      "content": null
    },
    {
      "path": "/ests/Old/sample.txt",
      "action": "CREATE",
      "content": "Замени меня, пожалуйста!"
    },
    {
      "path": "Tests/Old/sample.txt",
      "action": "REPLACE",
      "oldText": "old",
      "newText":  "new"
    }
  ],
  "delay": 1000,
  "baseDirectory": "./"
}
