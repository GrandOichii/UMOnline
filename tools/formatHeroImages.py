from PIL import Image, ImageDraw

IN = 'hero.webp'
OUT = 'hero-formatted.webp'

NAMES = [
    # 'Medusa',
    # 'Sinbad',
    # 'Alice',
    # 'King Arthur',
    # 'Bruce Lee',
    # 'Robin Hood',
    # 'Bigfoot',
    # 'InGen',
    # 'Raptors',
    # 'Sherlock Holmes',
    # 'Dracula',
    # 'Invisible Man',
    # 'Spike',
    # 'Willow'
    # 'Angel'
]

def format_hero(in_path, out_path):
    img = Image.open(in_path)
    mask = Image.new(mode='L', size=img.size, color=0)
    draw = ImageDraw.Draw(mask)
    # draw.ellipse((-img.size[0]/2, -img.size[1]/2, img.size[0]/2, img.size[1]/2), fill=255)
    draw.ellipse((0, 0, img.size[0], img.size[1]), fill=255)
    img.putalpha(mask)

    img.save(out_path)

for name in NAMES:
    in_path = f'../UMClient/assets/decks/{name}/heroes/hero.webp'
    out_path = f'../UMClient/assets/decks/{name}/heroes/{name}.webp'
    format_hero(in_path, out_path)
    print(f'Formatted {name}')
    
