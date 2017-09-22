# Copyright (c) 2016 Google Inc.

# Licensed under the Apache License, Version 2.0 (the "License"); you may not
# use this file except in compliance with the License. You may obtain a copy of
# the License at

# http://www.apache.org/licenses/LICENSE-2.0

# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
# WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
# License for the specific language governing permissions and limitations under
# the License.
from django.template.base import DebugLexer
import django.template.base as dtbase
import django.template.engine as dtengine
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
        + '{% include "another_template.html" with project="foo" file="/hello.cs" %}'
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
    engine = dtengine.Engine.get_default()
    for token in tokens:
        text = reconstruct_token(token)
        if token.token_type == dtbase.TOKEN_BLOCK:
            command = token.contents.split()[0]
            if command == 'include':
                try:
                    parser = dtbase.Parser([token], engine.template_libraries, 
                        engine.template_builtins, dtbase.Origin(text))
                    nodelist = parser.parse()
                    text = reconstruct_include_node(nodelist[0])
                except django.template.exceptions.TemplateSyntaxError, e:
                    # Unbalanced if, elif, etc.  The only blacks we're interested in
                    # need no balancing.
                    pass
        sys.stdout.write(text)
        


    
def reconstruct_token(token):
    """Given a token, reconstruct the original text."""
    # TOKEN_TEXT = 0
    # TOKEN_VAR = 1
    # TOKEN_BLOCK = 2
    # TOKEN_COMMENT = 3
    formats = ['%s', '{{%s}}', '{%% %s %%}', '{# %s #}']
    return formats[token.token_type] % token.contents 

def reconstruct_include_node(node):
    extra_context = " ".join(["%s=%s" % (item[0], item[1].token) 
            for item in node.extra_context.viewitems()])
    return '{%% include %s %s %%}' % (node.template.token, extra_context)

if __name__ == '__main__':
    main()
