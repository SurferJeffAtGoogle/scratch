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


def update_includes(template_string, template_out, repo_map):
    """Modifies a template and sets branch parameters for include blocks.

    Args:
        template_string (string):  The django template string.abs
        template_out (file):  The destination to write the modified template.
        repo_map (dict):  Maps repos to the branch to freeze them to.  Example:
           {
               'dotnet-docs-samples': 'ad3bda4b2d5fc47f3b1fb131c4d1f4323ab00db5',
               'nodejs-docs-samples': None  # Means remove branch= parameter.
           }
    """
    lexer = DebugLexer(template_string)
    tokens = lexer.tokenize()
    for token in tokens:
        text = reconstruct_token(token)
        include_node = to_include_node(token)
        if include_node:
            project = include_node.extra_context.get("project")
            if project in repo_map:
                # Update the include's branch statement.
                branch = repo_map[project]
                if not branch:
                    del include_node.extra_content["branch"]
                else:
                    include_node.extra_content["branch"] = branch
                text = reconstruct_include_node(include_node)
        template_out.write(text)
        

def to_include_node(token):    
    """Returns a dtbase.Node if this token represents an include node.

    Args:
        token (dtbase.Token): The token to examine.

    Returns:
        dtbase.Node: An IncludeNode for the token, or None is token is not
            an include node.
    """
    engine = dtengine.Engine.get_default()
    if token.token_type == dtbase.TOKEN_BLOCK:
        command = token.contents.split()[0]
        if command == 'include':
            parser = dtbase.Parser([token], engine.template_libraries, 
                engine.template_builtins, dtbase.Origin(token.contents))
            nodelist = parser.parse()
            return nodelist[0]

    
def reconstruct_token(token):
    """Given a token, reconstruct the original text."""
    # TOKEN_TEXT = 0
    # TOKEN_VAR = 1
    # TOKEN_BLOCK = 2
    # TOKEN_COMMENT = 3
    formats = ['%s', '{{%s}}', '{%% %s %%}', '{# %s #}']
    return formats[token.token_type] % token.contents


def reconstruct_include_node(node):
    """Given an include node, reconstruct the original text."""
    extra_context = " ".join(["%s=%s" % (item[0], item[1].token)
            for item in node.extra_context.viewitems()])
    if extra_context:
        return '{%% include %s with %s %%}' % (
            node.template.token, extra_context)
    else:
        return '{%% include %s %%}' % node.template.token


if __name__ == '__main__':
    for path in sys.argv[1:]:
        with open(path, 'rb') as f:
            text = f.read()
        with open(path, 'wb') as out:
            update_includes(text, out, {})

