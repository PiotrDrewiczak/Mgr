import sys
import json
import numpy as np
import largestinteriorrectangle as lir

def find_rectangles_in_shapes(shapes, shape_type):
    rectangles = []
    for i, shape in enumerate(shapes):
        try:
            shape_np = np.array([shape], dtype=np.int32)
            print(f"Processing {shape_type} (NumPy): {shape_np}", file=sys.stderr)
            rectangle = lir.lir(shape_np)
            print(f"Found rectangle: {rectangle}", file=sys.stderr)
            rectangles.append(rectangle.tolist())
        except Exception as e:
            print(f"Error finding largest inscribed rectangle in {shape_type}: {e}", file=sys.stderr)
            rectangles.append([])
    return rectangles

def main():
    input_data = sys.stdin.read()
    data = json.loads(input_data)

    quadrilaterals = data.get('quadrilaterals', [])
    rectangles_from_quadrilaterals = find_rectangles_in_shapes(quadrilaterals, "quadrilateral")

    triangles = data.get('unpairedTriangles', [])
    rectangles_from_triangles = find_rectangles_in_shapes(triangles, "triangle")

    output_data = json.dumps({
        "rectangles": rectangles_from_quadrilaterals + rectangles_from_triangles
    })
    print(output_data)

if __name__ == "__main__":
    main()
