#use fitz import to extract  from a pdf by rendering each page
import fitz  # PyMuPDF


import pytesseract
from PIL import Image
import io

pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"

def extract_text_from_pdf(pdf_path):
    """Extracts text from a PDF by rendering each page using PyMuPDF."""
    doc = fitz.open(pdf_path)
    all_text = []

    for page_num in range(len(doc)):
        page = doc.load_page(page_num)
        pix = page.get_pixmap(dpi=300)  # Render page as image
        img = Image.open(io.BytesIO(pix.tobytes("png")))
        text = pytesseract.image_to_string(img)

        # Look for pages containing specific text
        if "Course Evaluation" in text:
            all_text.append(f"\n--- Page {page_num + 1} ---\n{text}")

    doc.close()
    return "\n".join(all_text)

def extract_text_from_image(image_path):
    """Extracts text from a single image file."""
    try:
        img = Image.open(image_path)
        text = pytesseract.image_to_string(img)
        return text
    except Exception as e:
        return f"[ERROR] Image OCR failed: {e}"
    
# Example usage:
if __name__ == "__main__":
    
    # output test from pdf
    pdf_text = extract_text_from_pdf(r"MEC511-Outline.pdf")
    
    print(pdf_text)

    #output test from image
    #pdf_text2 = extract_text_from_image(r"elecourseoutline.png")
    #print(pdf_text2)
