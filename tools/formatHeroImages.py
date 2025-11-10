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
    # 'Buffy'
    # 'Beowulf',
    # 'Little Red Riding Hood',
    # 'Bloody Mary',
    # 'Achilles',
    # 'Sun Wukong',
    # 'Yennenga',
    # 'Luke Cage'
    # 'Moon Knight'
    # 'Ghost Rider',
    # 'Elektra',
    # 'Bullseye',
    # 'Daredevil',
    # 'Dr. Ellie Sattler',
    # 'The Genie',
    # 'Harry Houdini',
    # 'Ms. Marvel',
    # 'Cloak and Dagger',
    # 'Squirrel Girl',
    # 'Winter Soldier',
    # 'Black Widow',
    # 'Black Panther',
    # 'Spider-Man',
    # 'She-Hulk',
    # 'Doctor Strange',
    # 'Annie Christmas',
    # 'Nikola Tesla',
    # 'Golden Bat',
    # 'Dr. Jill Trent',
    # 'Tomoe Gozen',
    # 'Oda Nobunaga',
    # 'Hamlet',
    # 'Titania',
    # 'The Wayward Sisters',
    # 'William Shakespeare',
    # 'Eredin',
    # 'Philippa',
    # 'Yennefer & Triss',
    # 'Ciri',
    # 'Ancient Leshen',
    # 'Geralt of Rivia',
    # 'Raphael',
    # 'Michelangelo',
    # 'Leonardo',
    # 'Donatello',
    # 'Loki',
    # 'Pandora',
    # 'Chupacabra',
    # 'Blackbeard',
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
    
