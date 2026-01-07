import pytesseract  # OCR text extraction
from PIL import Image  # image loading for OCR
import io
import os
import re

# macOS Tesseract path
pytesseract.pytesseract.tesseract_cmd = "/opt/homebrew/bin/tesseract"

# Robust path to course_outline folder
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
folder_path_course_outline = os.path.join(BASE_DIR, "..", "course_outline")


# Key strings to look for when scanning
pages_key_words = ["Evaluation & Feedback", "Course Evaluation", "Evaluation"]
bad_string = {"|"}
exam_strings = {"exam", "examination", "test"}

def extract_text_from_image(file_path):
    """
    Extract each page of the course outline image(pdf/png) 
    """
    ext = file_path.lower().split(".")[-1]


    if ext in {"png", "jpg", "jpeg"}:
        return extract_text_from_image(file_path)

    else:
        print("wrong file path")
        return None 

def get_marks_split(text):


    assert isinstance(text, str), f"Expected string, got {type(text)}"

    marks = {}

    # Words that indicate PASSING REQUIREMENTS, not grade weights 
    reject_words = {
        "pass", "passing", "must", "minimum",
        "require", "required", "achieve",
        "overall", "to pass"
    }

    # Helper to parse percentages 
    def parse_percent(tokens, idx):

        # removes/seperates puncuation and brackets beside percentage
        t = tokens[idx].strip("(),.")
        # Case 1: "25%"
        if t.endswith("%"):
            try:
                return float(t[:-1])
            except ValueError:
                return None
        # Case 2: "25" "%"
        if idx + 1 < len(tokens) and tokens[idx + 1] == "%":
            try:
                return float(t)
            except ValueError:
                return None
        return None

    # Split OCR text into cleaned lines
    lines = [
        line.lower().replace("|", " ").strip()
        for line in text.split("\n")
        if line.strip()
    ]

    quiz_candidates = []

    for line in lines:

        # Ignore requirement sentences ie needs 50% requirement to pass
        if any(w in line for w in reject_words):
            continue

        tokens = line.split()

        # MIDTERM
        if "midterm" in line:
            val = None

            # First try token-based parsing
            if "%" in line:
                for i in range(len(tokens)):
                    val = parse_percent(tokens, i)
                    if val is not None:
                        break

            # Fallback: regex (handles "(36%)") 
            if val is None:
                val = extract_percent_from_line(line)

            if val is not None:
                marks["midterm"] = val

                

        # FINAL EXAM
      # FINAL EXAM
        if "final" in line and any(x in line for x in exam_strings):
            val = None

            if "%" in line:
                for i in range(len(tokens)):
                    val = parse_percent(tokens, i)
                    if val is not None:
                        break

            # Fallback: regex
            if val is None:
                val = extract_percent_from_line(line)

            if val is not None:
                marks["final"] = val



        # QUIZZES
        if "quiz" in line:
            val = None

            if "%" in line:
                for i in range(len(tokens)):
                    v = parse_percent(tokens, i)
                    if v is not None:
                        quiz_candidates.append(v)

            # Regex fallback
            v = extract_percent_from_line(line)
            if v is not None:
                quiz_candidates.append(v)


    # Resolve quizzes (prefer total over breakdown)
    if quiz_candidates:
        marks["quizzes"] = max(quiz_candidates)

    total = (
        marks.get("midterm", 0)
        + marks.get("final", 0)
        + marks.get("quizzes", 0)
    )

    if 0 < total <= 100:
        marks["assignments"] = round(100 - total, 1)
        return marks

    return None

def get_coursename(File_name):
    """
    Get course code from file name.
    """

    if len(File_name) <= 10:
        return "INVALID"

    course_code = ""

    for i in range(0, 3):
        if not File_name[i].isalpha():
            return None
        course_code += File_name[i]

    for i in range(3, 6):
        if not File_name[i].isdigit():
            return None
        course_code += File_name[i]

    return course_code


# matching ocr if there are bullet points instead
def extract_percent_from_line(line):
    """
    Matches:
    - (36%)
    - 36%
    - 36 % 
    """
    # use regex to find values wrapped in paranthesis
    match = re.search(r"\(?\b(\d+(\.\d+)?)\s*%\)?", line)
    if match:
        return float(match.group(1))
    return None

if __name__=="__main__":

    try:
        files = os.listdir(folder_path_course_outline)
        image_file = files[0]


    except Exception:
        print(" No image files found in course_outline folder.")
        exit()

    image_file_path = os.path.join(folder_path_course_outline, image_file)

    if not image_file.lower().endswith("png" or "jpg" or "jpeg"):
        print("File is not a image")
        exit()


    get_marks=get_marks_split(image_file)
    image_text = extract_text_from_image(image_file_path)
    get_outline=get_coursename(image_file)
    
    if image_text == "INVALID":
        print("Could not find evaluation section in image.")
        exit()

    #text_array = pdf_text.split()
    mark_split_dict = get_marks_split(image_text)

    print("✅ Grade split:", mark_split_dict)
    print("✅ Course code:", Course_code)



    