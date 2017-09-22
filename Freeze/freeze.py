# Hello
from django.template.base import Template
from django.template.engine import Engine
from pprint import pprint

def main():
    engine = Engine()
    engine.debug = True
    t = Template("Hello World", engine=engine)
    nodelist = t.compile_nodelist()
    pprint(nodelist)
    help(nodelist[0])

if __name__ == '__main__':
    main()
