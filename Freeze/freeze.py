# Hello
from django.template.base import DebugLexer
import django.template.base as dtbase
from django.conf import settings
import django
from pprint import pprint
import sys


TEMPLATES = [
    {
        'BACKEND': 'django.template.backends.django.DjangoTemplates',
    }
]
settings.configure(TEMPLATES=TEMPLATES)
django.setup()    


def main():
    s = ('<html>{% if test %}<h1>{{ varvalue }}</h1>{% endif %}\n'
        + '{% include "another_template.html" with a="b" and c="d" %}'
        + '</html>\n'
        + '{# cheesy #}\n'
        + '{% comment %}\n'
        + 'You need a new hairstyle.\n'
        + '{% endcomment %}\n')
    # path = sys.argv[1]
    # s = open(path, 'rb').read()
    lexer = DebugLexer(s)
    tokens = lexer.tokenize()
    if False:
        for token in tokens:
            pprint(vars(token))
            pprint(token.split_contents())
        print
    if True: # with open(path, 'wb') as out:
        for token in tokens:
            text = reconstruct(token)
            if token.token_type == dtbase.TOKEN_BLOCK:
                t = dtbase.Template(text)
                nodelist = t.nodelist
            sys.stdout.write(text)
        


    
def reconstruct(token):
    """Given a token, reconstruct the original text."""
    # TOKEN_TEXT = 0
    # TOKEN_VAR = 1
    # TOKEN_BLOCK = 2
    # TOKEN_COMMENT = 3
    formats = ['%s', '{{%s}}', '{%% %s %%}', '{# %s #}']
    return formats[token.token_type] % token.contents 

if __name__ == '__main__':
    main()
