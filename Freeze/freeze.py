# Hello
from django.template.base import DebugLexer
from pprint import pprint

def main():
    s = ('<html>{% if test %}<h1>{{ varvalue }}</h1>{% endif %}\n'
        + '{% include "another_template.html" with a="b" and c="d" %}'
        +  '</html>')
    lexer = DebugLexer(s)
    for token in lexer.tokenize():
        pprint(vars(token))

if __name__ == '__main__':
    main()
