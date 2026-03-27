import json
import sys

MOD = 256


def encodeJson(dct):
    corr = {str(i): x for i, x in enumerate('2053769418')}
    j = [corr.get(x, x) for x in json.dumps(dct, ensure_ascii=True)]
    encode = [chr((ord(x) * 7 + (i % 5)) % MOD) for i, x in enumerate(j)]
    return ''.join(encode)


def decodeJson(s):
    corr = {x: str(i) for i, x in enumerate('2053769418')}
    decode = [
        chr((ord(x) - (i % 5)) * pow(7, -1, MOD) % MOD)
        for i, x in enumerate(s)
    ]
    j = [corr.get(x, x) for x in decode]
    return json.loads(''.join(j))


def encode_file(input_file, output_file):
    with open(input_file, 'r', encoding='utf-8') as f:
        data = json.load(f)

    encoded = encodeJson(data)

    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(encoded, f)


def decode_file(input_file, output_file):
    with open(input_file, 'r', encoding='utf-8') as f:
        encoded = json.load(f)

    decoded = decodeJson(encoded)

    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(decoded, f, indent=2, ensure_ascii=False)


if __name__ == "__main__":

    if len(sys.argv) != 4:
        print("Usage:")
        print("  python encode_json.py encode input.json output.json")
        print("  python encode_json.py decode input.json output.json")
        sys.exit(1)

    mode = sys.argv[1]
    input_file = sys.argv[2]
    output_file = sys.argv[3]

    if mode == "encode":
        encode_file(input_file, output_file)
        print("Encode completed")

    elif mode == "decode":
        decode_file(input_file, output_file)
        print("Decode completed")

    else:
        print("Unknown mode:", mode)