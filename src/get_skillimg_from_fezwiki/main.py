import requests
import re
import os
import glob
import pprint
import cv2
import numpy as np
from PIL import Image
from bs4 import BeautifulSoup

root_dir = "imgs"
urls = [
    ("ソーサラー", 'http://fewiki.jp/index.php?Skill%2F%A5%BD%A1%BC%A5%B5%A5%E9%A1%BC%A5%B9%A5%AD%A5%EB'),
    ('セスタス', 'http://fewiki.jp/index.php?Skill%2F%A5%BB%A5%B9%A5%BF%A5%B9%A5%B9%A5%AD%A5%EB'),
    ('フェンサー', 'http://fewiki.jp/index.php?Skill%2F%A5%D5%A5%A7%A5%F3%A5%B5%A1%BC%A5%B9%A5%AD%A5%EB'),
    ('スカウト', 'http://fewiki.jp/index.php?Skill%2F%A5%B9%A5%AB%A5%A6%A5%C8%A5%B9%A5%AD%A5%EB'),
    ('ウォーリアー', 'http://fewiki.jp/index.php?Skill%2F%A5%A6%A5%A9%A5%EA%A5%A2%A1%BC%A5%B9%A5%AD%A5%EB'),
]

def download_file(url, dst_path):
    try:
        response = requests.get(url)
        image = response.content
        with open(dst_path, "wb") as f:
            f.write(image)
    except Exception as e:
        print("ERROR")
        print(e)

def download_skill_images(work_name, url):
    response = requests.get(url)
    soup = BeautifulSoup(response.text, 'html.parser')
    imgs = soup.findAll("img")

    dir = os.path.join(root_dir, work_name)
    if not os.path.exists(dir):
        os.makedirs(dir)

    for img in imgs:
        title = img.attrs["title"].lower()
        src   = img.attrs["src"]
        if re.search("gif", title) or re.search("png", title) or re.search("jpg", title) or re.search("jpeg", title) or re.search("bmp", title):
            print("download:%s" % title)
            download_file(src, os.path.join(dir, title))

def fix_alpha():
    files = glob.glob(root_dir + "/**/*")

    #file = files[0]
    file = os.path.abspath("temp.gif")
    img = Image.open(file).convert('RGBA')
    size = img.size
    for x in range(size[0]):
        for y in range(size[1]):
            r, g, b, a = img.getpixel((x,y))
            if r == 255 and g == 255 and b == 255:
                a = 0
            img.putpixel((x, y), (r, g, b, a))

    img.save("convert.png")


# download skill images
# for url in urls:
#     download_skill_images(url[0], url[1])

fix_alpha()