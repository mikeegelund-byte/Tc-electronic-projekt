import pdfplumber
import sys
from pathlib import Path

def read_pdf(pdf_path):
    """Extract all text from a PDF file"""
    print(f"\n{'='*80}")
    print(f"Reading: {pdf_path}")
    print(f"{'='*80}\n")
    
    try:
        with pdfplumber.open(pdf_path) as pdf:
            print(f"Total pages: {len(pdf.pages)}\n")
            
            full_text = []
            for i, page in enumerate(pdf.pages, 1):
                text = page.extract_text()
                if text:
                    print(f"--- Page {i} ---")
                    print(text)
                    print(f"\n")
                    full_text.append(text)
            
            return "\n\n".join(full_text)
    except Exception as e:
        print(f"ERROR reading {pdf_path}: {e}")
        return None

if __name__ == "__main__":
    pdf_dir = Path(r"d:\Tc electronic projekt\Tc originalt materiale")
    
    # List of PDFs to read
    pdf_files = [
        "Nova System Sysex Map.pdf",
        "TC Nova Manual.pdf",
        "novasystem-1_2-installation-guide.pdf",
        "nova-system-preset-list-473124.pdf"
    ]
    
    output_dir = Path(r"d:\Tc electronic projekt\Tc originalt materiale\extracted_text")
    output_dir.mkdir(exist_ok=True)
    
    for pdf_file in pdf_files:
        pdf_path = pdf_dir / pdf_file
        if pdf_path.exists():
            text = read_pdf(pdf_path)
            if text:
                # Save extracted text
                output_file = output_dir / f"{pdf_path.stem}.txt"
                with open(output_file, 'w', encoding='utf-8') as f:
                    f.write(text)
                print(f"\nSaved extracted text to: {output_file}")
        else:
            print(f"File not found: {pdf_path}")
