import mistune

class LinkCheckingRenderer(mistune.Renderer):
    def link(self, link, title, content):
        print(link)

renderer = LinkCheckingRenderer(escape=True, hard_wrap=True)
markdown = mistune.Markdown(renderer=renderer)
text = '''I am using **mistune markdown parser**.
(Google)[https://www.google.com/]
'''
result = markdown(text)
print(result)
