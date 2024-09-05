import sys
import json
import numpy as np
import largestinteriorrectangle as lir
import matplotlib.pyplot as plt

def plot_and_save_quadrilateral_with_rectangle(quad, rectangle, index):
    plt.figure()

    
    quad_np = np.array(quad + [quad[0]])  
    plt.plot(quad_np[:, 0], quad_np[:, 1], 'b-', label='Quadrilateral')
    plt.fill(quad_np[:, 0], quad_np[:, 1], 'lightblue', alpha=0.5)
    plt.scatter(quad_np[:, 0], quad_np[:, 1], color='red')

    
    if len(rectangle) > 0:
        rect_np = np.array(rectangle).reshape((2, 2))
        rect_corners = np.array([
            [rect_np[0, 0], rect_np[0, 1]],
            [rect_np[1, 0], rect_np[0, 1]],
            [rect_np[1, 0], rect_np[1, 1]],
            [rect_np[0, 0], rect_np[1, 1]],
            [rect_np[0, 0], rect_np[0, 1]]
        ])
        plt.plot(rect_corners[:, 0], rect_corners[:, 1], 'g-', label='Largest Inscribed Rectangle')
        plt.fill(rect_corners[:, 0], rect_corners[:, 1], 'lightgreen', alpha=0.5)
        plt.scatter(rect_corners[:, 0], rect_corners[:, 1], color='blue')

    plt.legend()
    plt.gca().invert_yaxis() 
    plt.axis('equal')
    
    
    filename = f"quadrilateral_{index}.png"
    plt.savefig(filename)
    plt.close()

def find_rectangles_in_quadrilaterals(quadrilaterals):
    rectangles = []
    for i, quad in enumerate(quadrilaterals):
        try:
            quad_np = np.array([quad], dtype=np.int32)
            print(f"Processing quadrilateral (NumPy): {quad_np}", file=sys.stderr)
            rectangle = lir.lir(quad_np)
            print(f"Found rectangle: {rectangle}", file=sys.stderr)
            rectangles.append(rectangle.tolist())

            
            plot_and_save_quadrilateral_with_rectangle(quad, rectangle.tolist(), i)

        except Exception as e:
            print(f"Error finding largest inscribed rectangle: {e}", file=sys.stderr)
            rectangles.append([])
    return rectangles

def main():
    input_data = sys.stdin.read()
    data = json.loads(input_data)
    quadrilaterals = data.get('quadrilaterals', [])
    rectangles = find_rectangles_in_quadrilaterals(quadrilaterals)
    output_data = json.dumps({
        "rectangles": rectangles
    })
    print(output_data)

if __name__ == "__main__":
    main()