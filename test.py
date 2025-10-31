import pytesseract
from PIL import Image 


pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"


img = Image.open(r"elecourseoutline.png")


text = pytesseract.image_to_string(img)

print("Extracted Text:")
print(text)
