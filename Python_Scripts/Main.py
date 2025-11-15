import fitz  #For PDF reading
import pytesseract # OCR text extraction
from PIL import Image #image loading for OCR
import io
import os
pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"


folder_path_course_outline = r".\course_outline"

#Key strings to look for when scanning
# - pages_key_words:String values in a course outline that indicate the page contains course grade details
# - bad_string:String values in a course outline that we want to ignore
# - exam_strings:String values that indicate the exam mark value is nearby in the text array
pages_key_words = ["Evaluation & Feedback", "Course Evaluation","Evaluation"]
bad_string = {"|"}
exam_strings = {"exam", "examination","test"}



def extract_text_from_pdf(pdf_path):
    """
    Extract each page of the course outline PDF file as an image and running OCR to return the first page
    containing the key words indicating that the page contains course grade details
    
    Parameters:
        pdf_path (str): Path to the input PDF file.
    
    Returns:
        str: Extracted page text OR an error message OR "INVALID PDF".
    """
    
    #open pdf document
    doc = fitz.open(pdf_path)

    #Loop through each page in the pdf 
    for page_num in range(len(doc)):

        try:
            #convert page to image
            page = doc.load_page(page_num)
            pix = page.get_pixmap(dpi=200)  
            img = Image.open(io.BytesIO(pix.tobytes("png")))

            #Run OCR on image to get string value
            text = pytesseract.image_to_string(img)
        
        except Exception as e:
            #Close pdf 
            doc.close()
            return f"[ERROR] PDF to image processing failed: {e}"

        # finding the page in the course outline containing the grade information
        for word in pages_key_words:
            if word in text:
                return text
            
    #No relevant page found
    doc.close()
    return "INVALID PDF"
    

def get_marks_split(text_array):

    """
    Scan through a text array containing information about the course outline grades and return a 
    dictionary containing the midterm,final, and assingments total percentage
    
    Parameters:
        text_array (list): list of every string word from the relevant pdf page containing the grade information
    
    Returns:
        Dict: {'midterm':(int), 'final':(int), 'assingments':(int)}
    """

    #Where the grade split will be stored
    marks_split = {}

    #Key words when scanning the text array
    marks_key_words = {"midterm","final"}
    
    #Loop through the text array until a key word is found
    for i in range(len(text_array)):
        
        if text_array[i].lower() in marks_key_words:

            #Check if after "final" its one of the exam_strings values, if it is not then we skip the loop interation since
            #the final grade value will not appear
            if text_array[i].lower() == "final" and text_array[i+1].lower() not in exam_strings:
                continue
            
            #temporary Mark split value
            temp_mark_split = 0
            #Counter to keep track how many strings are read after the key word was found
            counter = 0
            #index of where the key word was found
            j = i 

            #continue to move forward in the array after the key word was found until a string with a % is found
            while text_array[j][len(text_array[j])-1] != '%':
                #if the string containing % couldn't be found after reading 15 strings ahead of the key word then stop scanning
                if counter >15:
                    break
                j += 1
                counter += 1

            try:
                #once string containing % is found, remove % and convert string to integer to get the grade split value
                temp_mark_split = float(text_array[j].replace("%",""))

                rounded_mark_split = round(temp_mark_split,1)

                #store grade split value with corresponding grade split title into dictionary
                marks_split[text_array[i].lower()] = rounded_mark_split

                #remove the key word from the set of marks_key_words after it is found in the string
                marks_key_words.remove(text_array[i].lower())

                #if all the key words have been found in the string, then the remaining percentage is the grade split for assingments
                if len(marks_key_words) == 0:
                    marks_split["assignments"] =  100 - (marks_split["midterm"] + marks_split["final"])

                    #check if all the grade splits add up to 100%
                    if marks_split["assignments"] +  marks_split["midterm"] +  marks_split["final"] != 100:
                        return None
                    
                    return marks_split
            
            #incase converting the string to int gets an error
            except ValueError:
                continue
    
    #if all the key words have not been found then return None
    if marks_key_words != set():
            return None


def get_coursename(File_name):

    """
    Get the course code through the file name that is being scanned
    
    Parameters:
        File_name (string): String value of the file name
    
    Returns:
        str: course code value
    """
    
    #check if the length of the filename is atleast the sum of a coursecode(6) + .pdf(4) = 10
    if len(File_name)<=10:
        return "INVALID"
    
    course_code = ""

    #get the first 3 characters of the file name and check if they are alphabet values for the course code
    for i in range(0,3):
        if File_name[i].isalpha() == False:
            return None

        course_code += File_name[i]

     #get the next 3 characters of the file name and check if they are digit values for the course code
    for i in range(3,6):
        if File_name[i].isdigit() == False:
            return None

        course_code += File_name[i]

    return course_code




# Example usage:
if __name__ == "__main__":
    #get first file from course_outline folder
    pdf_file = os.listdir(folder_path_course_outline)[0]

    #full path to the pdf file
    pdf_file_path = folder_path_course_outline+"\\"+pdf_file

    #temporary string to check if the file is a pdf
    temp_string = ""

    #get the last 3 characters of the file name and check if it is "pdf" to ensure it is a pdf file
    for char in range(len(pdf_file_path)-3,len(pdf_file_path)):
        temp_string += pdf_file_path[char]
    
    if temp_string != "pdf":
        exit()

    #Get the course code from the file name
    Course_code = get_coursename(pdf_file)
    #Get the text of string from the page that contains the course grade split information
    pdf_text = extract_text_from_pdf(pdf_file_path)
    #split the string into an array
    text_array = pdf_text.split()
    #get the marks split of the course 
    mark_split_dict = get_marks_split(text_array)
    print(mark_split_dict)
    print(Course_code)


    
    

