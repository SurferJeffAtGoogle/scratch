import mistune

markdown = mistune.Markdown()
text = 'I am using **mistune markdown parser**'
result = markdown(text)
print(result)
