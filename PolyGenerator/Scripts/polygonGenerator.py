import sys
import json
import numpy as np
from polygenerator import random_polygon

def main(num_polygons, num_points, output_file):

    polygons = []
    for _ in range(num_polygons):
        polygon = random_polygon(num_points=num_points)
        polygons.append(polygon)

    scale_factor = 1000
    polygons_int = [
        np.array(
            [(int(x * scale_factor), int(y * scale_factor)) for x, y in polygon], 
            dtype=np.int32
        )
        for polygon in polygons
    ]

    with open(output_file, 'w') as f:
        json.dump({
            "polygons": [polygon.tolist() for polygon in polygons_int],
        }, f)

    print(f"{num_polygons} polygons data written to {output_file}")

if __name__ == "__main__":
    num_polygons = int(sys.argv[1])
    num_points = int(sys.argv[2])
    output_file = sys.argv[3]
    main(num_polygons, num_points, output_file)