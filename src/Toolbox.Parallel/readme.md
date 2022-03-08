# Toolbox.Parallel

Run commands in parallel based on standard input.

## Flags

| Flag         | Default | Description                                              |
|--------------|---------|----------------------------------------------------------|
| `--threads`  | 5       | Number of simultaneous threads                           |
| `--key`      | %s      | Placeholder key in the subcommand                        |
| `--out-file` | N/A     | If specified, also outputs each process stdout to a file |

## Examples

### Run a command for each line in a file

Create a file with some lines:

```text
# domains.txt
google.com
youtube.com
github.com
```

Pipe the contents into Parallel, write each result to a new file. By default, `%s` is the placeholder for the content that's being piped in.

```bash
cat ./domains.txt | parallel --out-file %s.html -- curl -L --max-redirs 5 -X GET %s
```