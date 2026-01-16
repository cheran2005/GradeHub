#import fitz  # PyMuPDF
#import os
import re


# Key strings to identify evaluation section
pages_key_words = ["evaluation & feedback", "course evaluation", "evaluation"]

exam_strings = {"exam", "examination", "test"}


# regex to extract the percentage AFTER a keyword
def extract_percent_after_keyword(line, keyword, all_keywords):
    start = line.find(keyword)
    if start == -1:
        return None

    start += len(keyword)

    # stop at the next keyword to avoid cross-matching
    end = len(line)
    for k in all_keywords:
        if k == keyword:
            continue
        pos = line.find(k, start)
        if pos != -1:
            end = min(end, pos)

    sub = line[start:end]

    # Supports: 30% , (30%) , (30)%
    match = re.search(r"\(?\s*(\d+(?:\.\d+)?)\s*\)?\s*%", sub)
    if match:
        return float(match.group(1))

    return None



def get_marks_split(text):

    assert isinstance(text, str), f"Expected string, got {type(text)}"

    marks = {}

    reject_words = {
        "pass", "passing", "must", "minimum",
        "require", "required", "achieve",
        "overall", "to pass"
    }

    lines = [
        line.lower().replace("|", " ").strip()
        for line in text.split("\n")
        if line.strip()
    ]

    quiz_candidates = []

    keywords = ["midterm", "final", "quiz", "assignment", "lab", "project"]

    for line in lines:

        if any(w in line for w in reject_words):
            continue

        # MIDTERM
        if "midterm" in line and "midterm" not in marks:
            val = extract_percent_after_keyword(line, "midterm", keywords)
            if val is not None:
                marks["midterm"] = val

        # FINAL
        if "final" in line and "final" not in marks:
            val = extract_percent_after_keyword(line, "final", keywords)
            if val is not None:
                marks["final"] = val

        # QUIZZES
        if "quiz" in line:
            val = extract_percent_after_keyword(line, "quiz", keywords)
            if val is not None:
                quiz_candidates.append(val)

        # ASSIGNMENTS / LABS / PROJECT
        for k in {"assignment", "lab", "project"}:
            if k in line and "assignments" not in marks:
                val = extract_percent_after_keyword(line, k, keywords)
                if val is not None:
                    marks["assignments"] = val

    if quiz_candidates:
        marks["quizzes"] = max(quiz_candidates)

    total = (
        marks.get("midterm", 0)
        + marks.get("final", 0)
        + marks.get("quizzes", 0)
    )

    if "assignments" not in marks and 0 < total <= 100:
        marks["assignments"] = round(100 - total, 1)

    return marks if marks else None





# COURSE CODE FROM TEXT

def get_coursename(text):
    text = text.upper()

    # 7 terms (C + 3 letters + 3 digits) → virtual chang school
    for i in range(len(text) - 6):
        if (
            text[i] == "C" and
            text[i+1].isalpha() and
            text[i+2].isalpha() and
            text[i+3].isalpha() and
            text[i+4].isdigit() and
            text[i+5].isdigit() and
            text[i+6].isdigit()
        ):
            return text[i:i+7]

    # 6 terms (3 letters + 3 digits) → regular course
    for i in range(len(text) - 5):
        if (
            text[i].isalpha() and
            text[i+1].isalpha() and
            text[i+2].isalpha() and
            text[i+3].isdigit() and
            text[i+4].isdigit() and
            text[i+5].isdigit()
        ):
            return text[i:i+6]

    return None


# Main
if __name__ == "__main__":

    # test case: virtual school course
    x = "for the course ccrm101, the final exam is weighted 25%, midterm is 18%"
    print("course code:", get_coursename(x))
    print("Grade split:", get_marks_split(x))

    print()

    # test case: regular course and test regex
    y = "for the course ele532, the final exam is (30)%, midterm exam is (50%)"
    print("course code:", get_coursename(y))
    print("Grade split:", get_marks_split(y))
