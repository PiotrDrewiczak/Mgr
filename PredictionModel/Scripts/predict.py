import sys
import json
import joblib
import numpy as np

def load_model(model_path):
    return joblib.load(model_path)

def predict(model, input_data):
    features = input_data["Features"]
    if not isinstance(features, list):
        raise ValueError("Features should be a list of floats.")
    
    data_array = np.array(features, dtype=float).reshape(1, -1)
    
    prediction = model.predict(data_array)
    return prediction[0]

if __name__ == "__main__":
    model_path = sys.argv[1]
    input_json_file = sys.argv[2]
    output_json_file = sys.argv[3]

    model = load_model(model_path)

    with open(input_json_file, 'r') as f:
        input_data = json.load(f)

    prediction = predict(model, input_data)
    
    prediction_result = {
        "Features": input_data["Features"],
        "CenterX": float(prediction[0]),
        "CenterY": float(prediction[1])
    }

    with open(output_json_file, 'w') as f:
        json.dump(prediction_result, f)
