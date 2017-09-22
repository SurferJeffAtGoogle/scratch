# Hello
from django.template.base import DebugLexer
import django.template.base as dtbase
from pprint import pprint
import sys

def main():
    s = ('<html>{% if test %}<h1>{{ varvalue }}</h1>{% endif %}\n'
        + '{% include "another_template.html" with a="b" and c="d" %}'
        + '</html>\n'
        + '{# cheesy #}\n'
        + '{% comment %}\n'
        + 'You need a new hairstyle.\n'
        + '{% endcomment %}\n')
    path = sys.argv[1]
    s = open(path, 'rb').read()
    lexer = DebugLexer(s)
    tokens = lexer.tokenize()
    if False:
        for token in tokens:
            pprint(vars(token))
            pprint(token.split_contents())
        print
    open(path, 'wb').write(''.join([reconstruct(token) for token in tokens]))


def reconstruct(token):
    functions = {
        dtbase.TOKEN_TEXT: lambda token: token.contents,
        dtbase.TOKEN_VAR: lambda token: '{{%s}}' % token.contents,
        dtbase.TOKEN_BLOCK: lambda token: '{%% %s %%}' % token.contents,
        dtbase.TOKEN_COMMENT: lambda token: '{# %s #}' % token.contents
    }
    return functions[token.token_type](token)

if __name__ == '__main__':
    main()
