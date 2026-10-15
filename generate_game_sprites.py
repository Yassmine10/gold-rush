import os
import math
from PIL import Image, ImageDraw, ImageFilter

BASE_DIR = r"C:\Users\lenovo\Downloads\Gold rush\Assets\Sprites"
CHAR_DIR = os.path.join(BASE_DIR, "Characters")
ITEM_DIR = os.path.join(BASE_DIR, "Items")
ENEMY_DIR = os.path.join(BASE_DIR, "Enemies")
ENV_DIR = os.path.join(BASE_DIR, "Environment")
UI_DIR = os.path.join(BASE_DIR, "UI")

for d in [CHAR_DIR, ITEM_DIR, ENEMY_DIR, ENV_DIR, UI_DIR]:
    os.makedirs(d, exist_ok=True)

# -------------------------------------------------------------
# HELPER FUNCTIONS
# -------------------------------------------------------------
def create_circle(draw, center, radius, fill, outline=None, width=1):
    x, y = center
    draw.ellipse([x - radius, y - radius, x + radius, y + radius], fill=fill, outline=outline, width=width)

def create_rounded_rect(draw, bbox, radius, fill, outline=None, width=1):
    draw.rounded_rectangle(bbox, radius=radius, fill=fill, outline=outline, width=width)

# -------------------------------------------------------------
# 1. CHARACTER SPRITES (Cartoon Hero matching Figma)
# -------------------------------------------------------------
def draw_character(skin="classic", pose="idle", size=(256, 256)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Palette
    skins_palette = {
        "classic": {
            "shirt": (59, 130, 246), "shirt_dark": (29, 78, 216),
            "pants": (29, 78, 216), "shoes": (30, 58, 138),
            "hair": (146, 64, 14), "hair_dark": (114, 48, 10),
            "skin": (252, 211, 77), "blush": (248, 113, 113, 120),
            "badge": (255, 215, 0), "extra": None
        },
        "adventurer": {
            "shirt": (16, 185, 129), "shirt_dark": (5, 150, 105),
            "pants": (4, 120, 87), "shoes": (6, 78, 59),
            "hair": (120, 53, 15), "hair_dark": (90, 40, 10),
            "skin": (252, 211, 77), "blush": (248, 113, 113, 120),
            "badge": (251, 191, 36), "extra": "feather"
        },
        "ninja": {
            "shirt": (239, 68, 68), "shirt_dark": (185, 28, 28),
            "pants": (153, 27, 27), "shoes": (69, 10, 10),
            "hair": (31, 41, 55), "hair_dark": (17, 24, 39),
            "skin": (252, 211, 77), "blush": (248, 113, 113, 120),
            "badge": (245, 158, 11), "extra": "headband"
        },
        "mage": {
            "shirt": (139, 92, 246), "shirt_dark": (109, 40, 217),
            "pants": (91, 33, 182), "shoes": (59, 7, 100),
            "hair": (196, 181, 253), "hair_dark": (167, 139, 250),
            "skin": (254, 230, 138), "blush": (248, 113, 113, 120),
            "badge": (167, 139, 250), "extra": "cape"
        },
        "king": {
            "shirt": (245, 158, 11), "shirt_dark": (217, 119, 6),
            "pants": (180, 83, 9), "shoes": (120, 53, 15),
            "hair": (251, 191, 36), "hair_dark": (217, 119, 6),
            "skin": (254, 243, 199), "blush": (248, 113, 113, 120),
            "badge": (255, 255, 255), "extra": "crown"
        }
    }
    pal = skins_palette[skin]

    cx, cy = 128, 135

    # 1. Ombre au sol (Drop Shadow)
    draw.ellipse([cx - 48, 225, cx + 48, 245], fill=(0, 0, 0, 60))

    # Cape pour Mage / Roi
    if pal["extra"] == "cape" or pal["extra"] == "crown":
        cape_col = (109, 40, 217) if pal["extra"] == "cape" else (185, 28, 28)
        draw.polygon([(cx - 36, cy + 10), (cx + 36, cy + 10), (cx + 46, 210), (cx - 46, 210)], fill=cape_col)

    # 2. Jambes et Pieds
    leg_offset = 0
    if pose == "run1":
        leg_offset = 12
    elif pose == "run2":
        leg_offset = -12

    # Jambe gauche
    draw.rounded_rectangle([cx - 30 - leg_offset, cy + 45, cx - 10 - leg_offset, cy + 85], radius=8, fill=pal["pants"])
    draw.ellipse([cx - 35 - leg_offset, cy + 78, cx - 5 - leg_offset, cy + 96], fill=pal["shoes"])

    # Jambe droite
    draw.rounded_rectangle([cx + 10 + leg_offset, cy + 45, cx + 30 + leg_offset, cy + 85], radius=8, fill=pal["pants"])
    draw.ellipse([cx + 5 + leg_offset, cy + 78, cx + 35 + leg_offset, cy + 96], fill=pal["shoes"])

    # 3. Torse (Tee-shirt)
    draw.rounded_rectangle([cx - 38, cy + 5, cx + 38, cy + 55], radius=16, fill=pal["shirt"], outline=pal["shirt_dark"], width=3)
    # Ombre bas du t-shirt
    draw.rounded_rectangle([cx - 36, cy + 45, cx + 36, cy + 54], radius=6, fill=pal["shirt_dark"])

    # Badge étoile sur la poitrine
    bx, by = cx, cy + 26
    badge_pts = [
        (bx, by - 12), (bx + 3, by - 4), (bx + 11, by - 4), (bx + 5, by + 1),
        (bx + 7, by + 9), (bx, by + 4), (bx - 7, by + 9), (bx - 5, by + 1),
        (bx - 11, by - 4), (bx - 3, by - 4)
    ]
    draw.polygon(badge_pts, fill=pal["badge"])

    # 4. Bras et Mains
    arm_y_off = -18 if pose == "jump" else 0
    # Bras gauche
    draw.rounded_rectangle([cx - 52, cy + 12 + arm_y_off, cx - 34, cy + 48 + arm_y_off], radius=9, fill=pal["shirt"])
    create_circle(draw, (cx - 43, cy + 54 + arm_y_off), 11, pal["skin"])
    # Bras droit
    draw.rounded_rectangle([cx + 34, cy + 12 - arm_y_off, cx + 52, cy + 48 - arm_y_off], radius=9, fill=pal["shirt"])
    create_circle(draw, (cx + 43, cy + 54 - arm_y_off), 11, pal["skin"])

    # 5. Cou et Tête
    draw.rounded_rectangle([cx - 12, cy - 8, cx + 12, cy + 10], radius=6, fill=pal["skin"])
    create_circle(draw, (cx, cy - 35), 44, pal["skin"], outline=(217, 119, 6, 80), width=2)

    # 6. Joues roses (Blush)
    draw.ellipse([cx - 36, cy - 30, cx - 18, cy - 18], fill=pal["blush"])
    draw.ellipse([cx + 18, cy - 30, cx + 36, cy - 18], fill=pal["blush"])

    # 7. Yeux expressifs
    if pose == "hurt":
        # Yeux étourdis (croix ou spirales)
        draw.line([(cx - 24, cy - 42), (cx - 10, cy - 28)], fill=(30, 27, 75), width=4)
        draw.line([(cx - 10, cy - 42), (cx - 24, cy - 28)], fill=(30, 27, 75), width=4)
        draw.line([(cx + 10, cy - 42), (cx + 24, cy - 28)], fill=(30, 27, 75), width=4)
        draw.line([(cx + 24, cy - 42), (cx + 10, cy - 28)], fill=(30, 27, 75), width=4)
    else:
        # Grand oeil blanc gauche
        create_circle(draw, (cx - 18, cy - 36), 13, (255, 255, 255))
        # Pupille noire
        create_circle(draw, (cx - 16, cy - 36), 7, (30, 27, 75))
        # Reflet blanc spéculaire
        create_circle(draw, (cx - 18, cy - 39), 3, (255, 255, 255))

        # Grand oeil blanc droit
        create_circle(draw, (cx + 18, cy - 36), 13, (255, 255, 255))
        create_circle(draw, (cx + 16, cy - 36), 7, (30, 27, 75))
        create_circle(draw, (cx + 14, cy - 39), 3, (255, 255, 255))

    # 8. Sourire
    if pose == "hurt":
        draw.arc([cx - 14, cy - 22, cx + 14, cy - 8], start=180, end=360, fill=(120, 53, 15), width=3)
    else:
        draw.arc([cx - 16, cy - 30, cx + 16, cy - 12], start=0, end=180, fill=(120, 53, 15), width=4)

    # 9. Cheveux volumineux et stylisés
    draw.ellipse([cx - 45, cy - 80, cx + 45, cy - 45], fill=pal["hair"])
    draw.ellipse([cx - 50, cy - 68, cx - 24, cy - 35], fill=pal["hair"])
    draw.ellipse([cx + 24, cy - 68, cx + 50, cy - 35], fill=pal["hair"])
    draw.ellipse([cx - 20, cy - 82, cx + 20, cy - 52], fill=pal["hair_dark"])

    # 10. Accessoires spécifiques (Couronne, Bandeau, Plume)
    if pal["extra"] == "crown":
        crown_pts = [
            (cx - 32, cy - 68), (cx - 20, cy - 95), (cx, cy - 75),
            (cx + 20, cy - 95), (cx + 32, cy - 68), (cx + 26, cy - 60), (cx - 26, cy - 60)
        ]
        draw.polygon(crown_pts, fill=(255, 215, 0), outline=(180, 83, 9), width=2)
        create_circle(draw, (cx, cy - 66), 4, (239, 68, 68)) # Rubis
        create_circle(draw, (cx - 18, cy - 66), 3, (59, 130, 246)) # Saphir
        create_circle(draw, (cx + 18, cy - 66), 3, (59, 130, 246))
    elif pal["extra"] == "headband":
        draw.rectangle([cx - 44, cy - 54, cx + 44, cy - 44], fill=(185, 28, 28))
        draw.polygon([(cx + 38, cy - 48), (cx + 56, cy - 38), (cx + 42, cy - 32)], fill=(185, 28, 28))
    elif pal["extra"] == "feather":
        feather_pts = [(cx + 26, cy - 60), (cx + 46, cy - 90), (cx + 32, cy - 70)]
        draw.polygon(feather_pts, fill=(239, 68, 68))

    return img

# -------------------------------------------------------------
# 2. COIN SPRITES (High-Shine 3D/2D Gold Coins)
# -------------------------------------------------------------
def draw_coin(frame=0, size=(256, 256)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128

    # Facteur d'écrasement horizontal pour rotation 2.5D
    scale_x = [1.0, 0.65, 0.22, 0.65][frame]
    w = int(95 * scale_x)
    h = 95

    # 1. Halo lumineux doré
    if frame == 0:
        glow = Image.new("RGBA", size, (0, 0, 0, 0))
        gdraw = ImageDraw.Draw(glow)
        create_circle(gdraw, (cx, cy), 115, (255, 215, 0, 80))
        create_circle(gdraw, (cx, cy), 125, (255, 235, 100, 40))
        glow = glow.filter(ImageFilter.GaussianBlur(8))
        img.paste(glow, (0, 0), glow)

    # 2. Tranche 3D de la pièce
    draw.ellipse([cx - w + 6, cy - h, cx + w + 6, cy + h], fill=(180, 83, 9))

    # 3. Disque d'or principal
    draw.ellipse([cx - w, cy - h, cx + w, cy + h], fill=(255, 215, 0), outline=(217, 119, 6), width=5)

    # 4. Anneau intérieur gravé
    if w > 20:
        inner_w = int(w * 0.76)
        inner_h = int(h * 0.76)
        draw.ellipse([cx - inner_w, cy - inner_h, cx + inner_w, cy + inner_h], fill=(254, 240, 138), outline=(234, 88, 12), width=3)

    # 5. Symbole "$" au centre
    if w > 35:
        # $ stylisé en or sombre/orangé
        draw.text((cx - 12, cy - 32), "$", fill=(180, 83, 9), font_size=55)

    # 6. Éclat brillant spéculaire (Gleam / Sparkle)
    if frame == 0:
        gleam_pts = [
            (cx - 45, cy - 45), (cx - 25, cy - 40), (cx - 40, cy - 25)
        ]
        draw.ellipse([cx - 55, cy - 55, cx - 25, cy - 30], fill=(255, 255, 255, 220))
        # Étoile brillante en haut à droite
        sx, sy = cx + 55, cy - 55
        star_pts = [(sx, sy - 14), (sx + 4, sy - 4), (sx + 14, sy), (sx + 4, sy + 4),
                    (sx, sy + 14), (sx - 4, sy + 4), (sx - 14, sy), (sx - 4, sy - 4)]
        draw.polygon(star_pts, fill=(255, 255, 255, 240))

    return img

# -------------------------------------------------------------
# 3. ENEMY SPRITES (Slime & Bat)
# -------------------------------------------------------------
def draw_slime(color_name="green", pose="idle", size=(256, 256)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140

    cols = {
        "green": {"main": (16, 185, 129), "light": (52, 211, 153), "dark": (5, 150, 105), "eye": (4, 120, 87)},
        "purple": {"main": (139, 92, 246), "light": (167, 139, 250), "dark": (109, 40, 217), "eye": (91, 33, 182)},
        "fire": {"main": (249, 115, 22), "light": (251, 146, 60), "dark": (234, 88, 12), "eye": (154, 52, 18)}
    }[color_name]

    rx, ry = (75, 55) if pose == "idle" else (90, 40)

    # Ombre
    draw.ellipse([cx - rx - 5, cy + ry - 12, cx + rx + 5, cy + ry + 15], fill=(0, 0, 0, 50))

    # Corps gélatineux (Base + dôme)
    draw.ellipse([cx - rx, cy - ry + 15, cx + rx, cy + ry], fill=cols["dark"])
    draw.ellipse([cx - rx + 3, cy - ry, cx + rx - 3, cy + ry - 8], fill=cols["main"])
    draw.ellipse([cx - rx + 10, cy - ry + 5, cx + rx - 10, cy + ry - 25], fill=cols["light"])

    # Reflet spéculaire doux translucide
    draw.ellipse([cx - 45, cy - ry + 15, cx - 10, cy - ry + 35], fill=(255, 255, 255, 120))

    # Yeux cartoon grands et mignons
    eye_y = cy - 5
    # Oeil gauche
    create_circle(draw, (cx - 24, eye_y), 16, (255, 255, 255))
    create_circle(draw, (cx - 22, eye_y), 9, cols["eye"])
    create_circle(draw, (cx - 25, eye_y - 3), 4, (255, 255, 255))
    # Oeil droit
    create_circle(draw, (cx + 24, eye_y), 16, (255, 255, 255))
    create_circle(draw, (cx + 22, eye_y), 9, cols["eye"])
    create_circle(draw, (cx + 19, eye_y - 3), 4, (255, 255, 255))

    # Petite bouche
    draw.arc([cx - 10, eye_y + 12, cx + 10, eye_y + 26], start=0, end=180, fill=cols["eye"], width=3)

    return img

def draw_bat(frame=0, size=(256, 256)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128

    wing_y = -25 if frame == 0 else 15

    # Ailes violettes
    wing_pts_l = [(cx - 20, cy), (cx - 95, cy + wing_y), (cx - 75, cy + 35), (cx - 45, cy + 20), (cx - 20, cy + 25)]
    draw.polygon(wing_pts_l, fill=(124, 58, 237), outline=(91, 33, 182), width=3)
    wing_pts_r = [(cx + 20, cy), (cx + 95, cy + wing_y), (cx + 75, cy + 35), (cx + 45, cy + 20), (cx + 20, cy + 25)]
    draw.polygon(wing_pts_r, fill=(124, 58, 237), outline=(91, 33, 182), width=3)

    # Corps pelucheux
    draw.ellipse([cx - 28, cy - 25, cx + 28, cy + 35], fill=(76, 29, 149), outline=(59, 7, 100), width=3)

    # Oreilles pointues
    draw.polygon([(cx - 22, cy - 20), (cx - 28, cy - 50), (cx - 8, cy - 25)], fill=(109, 40, 217))
    draw.polygon([(cx + 22, cy - 20), (cx + 28, cy - 50), (cx + 8, cy - 25)], fill=(109, 40, 217))

    # Yeux jaunes menaçants mais cartoons
    create_circle(draw, (cx - 12, cy - 5), 8, (253, 230, 138))
    create_circle(draw, (cx - 10, cy - 5), 4, (30, 27, 75))
    create_circle(draw, (cx + 12, cy - 5), 8, (253, 230, 138))
    create_circle(draw, (cx + 14, cy - 5), 4, (30, 27, 75))

    # Crocs blancs
    draw.polygon([(cx - 7, cy + 14), (cx - 4, cy + 22), (cx - 1, cy + 14)], fill=(255, 255, 255))
    draw.polygon([(cx + 1, cy + 14), (cx + 4, cy + 22), (cx + 7, cy + 14)], fill=(255, 255, 255))

    return img

# -------------------------------------------------------------
# 4. HAZARDS & EXITS (Spikes, Flag, Portal, Golden Gate)
# -------------------------------------------------------------
def draw_spikes(size=(512, 128)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    spike_count = 8
    sw = size[0] // spike_count

    # Base de roche
    draw.rectangle([0, size[1] - 25, size[0], size[1]], fill=(68, 64, 60))

    for i in range(spike_count):
        x = i * sw
        mid = x + sw // 2
        # Cône de pic métallique
        draw.polygon([(x + 2, size[1] - 20), (mid, 10), (x + sw - 2, size[1] - 20)], fill=(239, 68, 68), outline=(153, 27, 27), width=3)
        # Reflet métallique brillant sur la moitié gauche
        draw.polygon([(x + 5, size[1] - 20), (mid, 10), (mid, size[1] - 20)], fill=(254, 202, 202, 180))

    return img

def draw_exit_flag(size=(256, 512)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    cx = 100
    # Socle de pierre gravée
    draw.rounded_rectangle([cx - 45, 450, cx + 65, 495], radius=10, fill=(120, 113, 108), outline=(68, 64, 60), width=4)
    draw.rounded_rectangle([cx - 30, 435, cx + 50, 455], radius=6, fill=(168, 162, 158))

    # Mât en bois d'ébène
    draw.rounded_rectangle([cx - 6, 40, cx + 6, 440], radius=5, fill=(120, 53, 15), outline=(69, 26, 3), width=2)
    # Sommet doré
    create_circle(draw, (cx, 36), 14, (255, 215, 0), outline=(180, 83, 9), width=2)

    # Drapeau rouge ondulant
    flag_pts = [(cx + 6, 45), (cx + 140, 95), (cx + 6, 150)]
    draw.polygon(flag_pts, fill=(239, 68, 68), outline=(185, 28, 28), width=3)

    # Étoile dorée sur le drapeau
    fx, fy = cx + 50, 95
    star_pts = [(fx, fy - 18), (fx + 5, fy - 5), (fx + 18, fy), (fx + 5, fy + 5),
                (fx, fy + 18), (fx - 5, fy + 5), (fx - 18, fy), (fx - 5, fy - 5)]
    draw.polygon(star_pts, fill=(255, 215, 0))

    # Panneau en bois "SORTIE"
    draw.rounded_rectangle([cx - 55, 250, cx + 75, 305], radius=10, fill=(37, 99, 235), outline=(30, 58, 138), width=3)
    draw.text((cx - 38, 262), "SORTIE", fill=(255, 255, 255), font_size=24)

    return img

def draw_exit_portal(size=(512, 512)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256

    # Anneaux extérieurs d'énergie
    for r, alpha in [(230, 40), (200, 80), (170, 140)]:
        create_circle(draw, (cx, cy), r, None, outline=(167, 139, 250, alpha), width=12)

    # Vortex d'améthyste
    draw.ellipse([cx - 150, cy - 200, cx + 150, cy + 200], fill=(109, 40, 217), outline=(139, 92, 246), width=8)
    draw.ellipse([cx - 100, cy - 140, cx + 100, cy + 140], fill=(147, 51, 234))
    draw.ellipse([cx - 50, cy - 80, cx + 50, cy + 80], fill=(216, 180, 254))
    draw.ellipse([cx - 20, cy - 35, cx + 20, cy + 35], fill=(255, 255, 255))

    # Étoiles d'énergie tourbillonnantes
    for angle in range(0, 360, 45):
        rad = math.radians(angle)
        px = int(cx + 175 * math.cos(rad))
        py = int(cy + 175 * math.sin(rad))
        create_circle(draw, (px, py), 9, (255, 255, 255), outline=(167, 139, 250), width=2)

    return img

def draw_golden_gate(size=(512, 512)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256

    # Arche lumineuse céleste
    draw.arc([70, 40, 442, 420], start=180, end=360, fill=(255, 215, 0), width=36)
    draw.arc([90, 60, 422, 420], start=180, end=360, fill=(254, 240, 138), width=14)

    # Piliers d'or sculptés
    # Pilier gauche
    draw.rounded_rectangle([55, 180, 115, 490], radius=12, fill=(245, 158, 11), outline=(180, 83, 9), width=5)
    draw.rounded_rectangle([45, 470, 125, 505], radius=8, fill=(217, 119, 6))
    create_circle(draw, (85, 160), 22, (255, 215, 0), outline=(180, 83, 9), width=3)
    draw.text((75, 145), "⭐", fill=(255, 255, 255), font_size=20)

    # Pilier droit
    draw.rounded_rectangle([397, 180, 457, 490], radius=12, fill=(245, 158, 11), outline=(180, 83, 9), width=5)
    draw.rounded_rectangle([387, 470, 467, 505], radius=8, fill=(217, 119, 6))
    create_circle(draw, (427, 160), 22, (255, 215, 0), outline=(180, 83, 9), width=3)
    draw.text((417, 145), "⭐", fill=(255, 255, 255), font_size=20)

    # Halo radieux intérieur
    draw.ellipse([140, 200, 372, 490], fill=(254, 243, 199, 140))
    draw.text((cx - 45, 300), "VICTOIRE", fill=(217, 119, 6), font_size=22)

    return img

# -------------------------------------------------------------
# 5. ENVIRONMENT PLATFORMS & PROPS
# -------------------------------------------------------------
def draw_grass_platform(size=(512, 160)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Terre / Roche sous-jacente
    draw.rounded_rectangle([0, 30, size[0], size[1]], radius=16, fill=(120, 53, 15), outline=(69, 26, 3), width=4)

    # Détails de roche dans la terre
    for x in range(30, size[0], 80):
        draw.ellipse([x, 80, x + 35, 115], fill=(146, 64, 14))

    # Pelouse verte épaisse par-dessus
    draw.rounded_rectangle([0, 10, size[0], 55], radius=14, fill=(34, 197, 94), outline=(21, 128, 61), width=4)
    # Brins d'herbe supérieure
    for x in range(10, size[0] - 10, 24):
        draw.polygon([(x, 15), (x + 8, 0), (x + 16, 15)], fill=(74, 222, 128))

    return img

def draw_wood_platform(size=(512, 120)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Poutre de bois principale
    draw.rounded_rectangle([0, 20, size[0], size[1]], radius=14, fill=(146, 64, 14), outline=(114, 48, 10), width=4)

    # Lignes de rainures de bois
    for y in [45, 75, 100]:
        draw.line([(15, y), (size[0] - 15, y)], fill=(114, 48, 10), width=3)

    # Clous / boulons dorés
    for x in [30, size[0] - 30]:
        create_circle(draw, (x, 70), 8, (251, 191, 36), outline=(180, 83, 9), width=2)

    # Mousse verte sur le dessus
    draw.rounded_rectangle([0, 12, size[0], 35], radius=10, fill=(74, 222, 128))

    return img

def draw_crystal_platform(size=(512, 160)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Pierre sombre de caverne
    draw.rounded_rectangle([0, 30, size[0], size[1]], radius=16, fill=(45, 20, 80), outline=(25, 10, 45), width=4)

    # Cristaux violets incrustés sur le dessus
    for x in range(35, size[0] - 40, 70):
        # Cristal en losange
        c_pts = [(x, 30), (x + 15, 2), (x + 30, 30), (x + 15, 45)]
        draw.polygon(c_pts, fill=(167, 139, 250), outline=(124, 58, 237), width=2)
        # Reflet blanc
        draw.polygon([(x + 8, 28), (x + 15, 6), (x + 15, 28)], fill=(255, 255, 255, 200))

    # Liseré supérieur violet
    draw.rounded_rectangle([0, 25, size[0], 48], radius=8, fill=(109, 40, 217))

    return img

def draw_sky_cloud(size=(512, 256)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Nuage composé de cercles et ellipses
    draw.ellipse([40, 90, 220, 220], fill=(255, 255, 255, 230))
    draw.ellipse([140, 40, 360, 220], fill=(255, 255, 255, 245))
    draw.ellipse([280, 80, 470, 220], fill=(255, 255, 255, 230))
    draw.rounded_rectangle([70, 140, 440, 225], radius=35, fill=(240, 245, 255, 255))
    # Ombre sous le nuage
    draw.rounded_rectangle([100, 195, 410, 225], radius=15, fill=(203, 213, 225, 160))

    return img

# -------------------------------------------------------------
# 6. UI ASSETS (Hearts, Star, Coin Icon)
# -------------------------------------------------------------
def draw_ui_heart(filled=True, size=(128, 128)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 64, 60

    fill_col = (239, 68, 68) if filled else (75, 85, 99, 140)
    out_col = (185, 28, 28) if filled else (31, 41, 55, 180)

    # Deux cercles et un triangle inversé pour le coeur
    draw.ellipse([cx - 44, cy - 35, cx + 4, cy + 15], fill=fill_col)
    draw.ellipse([cx - 4, cy - 35, cx + 44, cy + 15], fill=fill_col)
    draw.polygon([(cx - 42, cy), (cx + 42, cy), (cx, cy + 48)], fill=fill_col)

    if filled:
        # Reflet spéculaire doux
        draw.ellipse([cx - 32, cy - 25, cx - 12, cy - 8], fill=(255, 255, 255, 180))

    return img

def draw_ui_star(filled=True, size=(128, 128)):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 64, 64

    fill_col = (255, 215, 0) if filled else (100, 116, 139, 160)
    out_col = (217, 119, 6) if filled else (51, 65, 85, 200)

    r_outer = 50
    r_inner = 22
    pts = []
    for i in range(10):
        r = r_outer if i % 2 == 0 else r_inner
        ang = math.radians(-90 + i * 36)
        pts.append((cx + int(r * math.cos(ang)), cy + int(r * math.sin(ang))))

    draw.polygon(pts, fill=fill_col, outline=out_col, width=3)
    if filled:
        create_circle(draw, (cx - 12, cy - 14), 7, (255, 255, 255, 220))

    return img

# -------------------------------------------------------------
# MAIN EXPORT
# -------------------------------------------------------------
print("Generation des assets 2D professionnels en cours...")

# 1. Characters
for skin in ["classic", "adventurer", "ninja", "mage", "king"]:
    draw_character(skin, "idle").save(os.path.join(CHAR_DIR, f"Player_{skin.capitalize()}_Idle.png"))
    if skin == "classic":
        draw_character(skin, "run1").save(os.path.join(CHAR_DIR, "Player_Classic_Run1.png"))
        draw_character(skin, "run2").save(os.path.join(CHAR_DIR, "Player_Classic_Run2.png"))
        draw_character(skin, "jump").save(os.path.join(CHAR_DIR, "Player_Classic_Jump.png"))
        draw_character(skin, "hurt").save(os.path.join(CHAR_DIR, "Player_Classic_Hurt.png"))

# 2. Coins
for f in range(4):
    draw_coin(f).save(os.path.join(ITEM_DIR, f"Coin_Frame{f}.png"))
draw_coin(0).save(os.path.join(ITEM_DIR, "Coin_Idle.png"))

# 3. Enemies
draw_slime("green", "idle").save(os.path.join(ENEMY_DIR, "Slime_Green_Idle.png"))
draw_slime("green", "squash").save(os.path.join(ENEMY_DIR, "Slime_Green_Squash.png"))
draw_slime("purple", "idle").save(os.path.join(ENEMY_DIR, "Slime_Purple_Idle.png"))
draw_slime("fire", "idle").save(os.path.join(ENEMY_DIR, "Slime_Fire_Idle.png"))
draw_bat(0).save(os.path.join(ENEMY_DIR, "Bat_Fly1.png"))
draw_bat(1).save(os.path.join(ENEMY_DIR, "Bat_Fly2.png"))

# 4. Hazards & Exits
draw_spikes().save(os.path.join(ENV_DIR, "Spikes_Hazard.png"))
draw_exit_flag().save(os.path.join(ENV_DIR, "Exit_Flag.png"))
draw_exit_portal().save(os.path.join(ENV_DIR, "Exit_Portal.png"))
draw_golden_gate().save(os.path.join(ENV_DIR, "Exit_GoldenGate.png"))

# 5. Platforms & Environment
draw_grass_platform().save(os.path.join(ENV_DIR, "Platform_Grass.png"))
draw_wood_platform().save(os.path.join(ENV_DIR, "Platform_Wood.png"))
draw_crystal_platform().save(os.path.join(ENV_DIR, "Platform_Crystal.png"))
draw_sky_cloud().save(os.path.join(ENV_DIR, "Cloud_Fluffy.png"))

# 6. UI
draw_ui_heart(True).save(os.path.join(UI_DIR, "UI_Heart_Full.png"))
draw_ui_heart(False).save(os.path.join(UI_DIR, "UI_Heart_Empty.png"))
draw_ui_star(True).save(os.path.join(UI_DIR, "UI_Star_Gold.png"))
draw_ui_star(False).save(os.path.join(UI_DIR, "UI_Star_Empty.png"))
draw_coin(0, (128, 128)).save(os.path.join(UI_DIR, "UI_Coin_Icon.png"))

print("SUCCESS: Tous les sprites 2D ont ete generes dans Assets/Sprites !")
