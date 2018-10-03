import mistune

renderer = mistune.Renderer(escape=True, hard_wrap=True)
markdown = mistune.Markdown(renderer=renderer)
text = 'I am using **mistune markdown parser**'
result = markdown(text)
print(result)
