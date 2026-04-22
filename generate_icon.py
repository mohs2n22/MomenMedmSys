"""
Generate a professional icon for MomenMedmSys - Medical Equipment Management System
Outputs: AppIcon.ico (256x256, 128x128, 64x64, 48x48, 32x32, 16x16)
"""
from PIL import Image, ImageDraw, ImageFont
import os
import math

def create_logo(size):
    """Create a professional medical equipment management logo"""
    img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Color scheme - Medical blue gradient
    primary_blue = (0, 102, 179)      # #0066B3
    light_blue = (0, 153, 229)        # #0099E5
    white = (255, 255, 255)
    accent_cyan = (0, 204, 255)       # #00CCFF
    
    # Background circle with gradient effect
    margin = int(size * 0.05)
    center = size // 2
    radius = size // 2 - margin
    
    # Outer circle (darker blue)
    draw.ellipse([margin, margin, size - margin, size - margin], 
                 fill=primary_blue)
    
    # Inner circle (lighter blue for depth)
    inner_margin = int(size * 0.1)
    draw.ellipse([inner_margin, inner_margin, size - inner_margin, size - inner_margin], 
                 fill=light_blue)
    
    # Medical cross (centered)
    cross_width = int(size * 0.12)
    cross_length = int(size * 0.28)
    cross_top = center - cross_length
    cross_left = center - cross_width // 2
    
    # Vertical bar
    draw.rectangle([cross_left, cross_top, 
                    cross_left + cross_width, cross_top + cross_length * 2], 
                   fill=white)
    # Horizontal bar
    draw.rectangle([center - cross_length, center - cross_width // 2,
                    center + cross_length, center + cross_width // 2], 
                   fill=white)
    
    # Gear teeth around the edge (equipment/technical feel)
    num_teeth = 8
    tooth_depth = int(size * 0.04)
    tooth_width = int(size * 0.06)
    
    for i in range(num_teeth):
        angle = (2 * math.pi * i) / num_teeth + math.pi / num_teeth
        x = center + int((radius - 2) * math.cos(angle))
        y = center + int((radius - 2) * math.sin(angle))
        
        # Draw small rectangular teeth
        tooth_left = x - tooth_width // 2
        tooth_top = y - tooth_width // 2
        draw.rectangle([tooth_left, tooth_top,
                        tooth_left + tooth_width, tooth_top + tooth_width],
                       fill=primary_blue)
    
    # "M" text at bottom (subtle branding)
    try:
        font_size = max(int(size * 0.15), 10)
        font = ImageFont.truetype("arial.ttf", font_size)
    except:
        font = ImageFont.load_default()
    
    # Skip text on small sizes
    if size >= 48:
        text = "M"
        bbox = draw.textbbox((0, 0), text, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]
        text_x = center - text_width // 2
        text_y = size - int(size * 0.18) - text_height
        draw.text((text_x, text_y), text, fill=white, font=font)
    
    return img

def main():
    # Create output directory
    output_dir = os.path.join(os.path.dirname(__file__), 'MomenMedmSys.WPF', 'Assets')
    os.makedirs(output_dir, exist_ok=True)
    
    # Generate multiple sizes for .ico
    sizes = [256, 128, 64, 48, 32, 16]
    images = []
    
    for size in sizes:
        img = create_logo(size)
        images.append(img)
        # Also save as PNG for reference
        png_path = os.path.join(output_dir, f'MomenMedmSys_{size}x{size}.png')
        img.save(png_path)
        print(f"Created {png_path}")
    
    # Save as multi-size .ico
    ico_path = os.path.join(output_dir, 'AppIcon.ico')
    images[0].save(ico_path, sizes=[(img.width, img.height) for img in images], 
                   format='ICO')
    print(f"\nCreated {ico_path}")
    
    # Copy to project root for easy access
    import shutil
    root_icon_path = os.path.join(os.path.dirname(__file__), 'AppIcon.ico')
    shutil.copy2(ico_path, root_icon_path)
    print(f"Copied to {root_icon_path}")

if __name__ == '__main__':
    main()
