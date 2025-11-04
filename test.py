"not to self utilize python3 -m venv venv to create a virtual environment"
"""include imports from pytesseract that will handle data"""
import pytesseract


from PIL import Image 


pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"


img = Image.open(r"elecourseoutline.png")

" using the image make it so that the image converts the text"
text = pytesseract.image_to_string(img)

print("Extracted Text:")
print(text)
