import pytesseract
import re
from PIL import Image 


pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"





img = Image.open(r"elecourseoutline.png")


text = pytesseract.image_to_string(img)

print("Extracted Text:")
token = re.split(r'[\s\n]+',text)
print(text)
print(token)
