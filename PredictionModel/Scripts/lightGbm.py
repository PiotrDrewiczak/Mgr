import sys
import json
import numpy as np
import joblib
import optuna
import lightgbm as lgb
from sklearn.multioutput import MultiOutputRegressor
from sklearn.model_selection import KFold
from sklearn.metrics import r2_score, mean_absolute_error

def objective(trial, features, center):
    param = {
    'objective': 'regression',
     'verbosity': -1, 
    'n_estimators': trial.suggest_int('n_estimators', 100, 1000),
    'learning_rate': trial.suggest_float('learning_rate', 0.01, 0.3),
    'max_depth': trial.suggest_int('max_depth', 3, 10),
    'num_leaves': trial.suggest_int('num_leaves', 20, 300),
    'subsample': trial.suggest_float('subsample', 0.5, 1.0),  # Zamiana suggest_uniform
    'colsample_bytree': trial.suggest_float('colsample_bytree', 0.5, 1.0)  # Zamiana suggest_uniform
}

    
    kf = KFold(n_splits=5, shuffle=True, random_state=42)
    r2_scores = []
    
    for train_index, valid_index in kf.split(features):
        train_features, valid_features = features[train_index], features[valid_index]
        train_center, valid_center = center[train_index], center[valid_index]
        
        model = MultiOutputRegressor(lgb.LGBMRegressor(**param))
        model.fit(train_features, train_center)
        
        predicted_center = model.predict(valid_features)
        score = r2_score(valid_center, predicted_center, multioutput='uniform_average')
        r2_scores.append(score)
    
    return np.mean(r2_scores)

def train_lightgbm_multioutput(data):
    features = np.array([item["Features"] for item in data])
    center = np.array([[item["CenterX"], item["CenterY"]] for item in data])

    study = optuna.create_study(direction='maximize')
    study.optimize(lambda trial: objective(trial, features, center), timeout=3600)
    
    best_params = study.best_params
    
    print("Najlepsze hiperparametry:", best_params)

    kf = KFold(n_splits=5, shuffle=True, random_state=42)
    r2_scores = []
    mae_scores = []

    for train_index, valid_index in kf.split(features):
        train_features, valid_features = features[train_index], features[valid_index]
        train_center, valid_center = center[train_index], center[valid_index]
        
        model = MultiOutputRegressor(lgb.LGBMRegressor(**best_params))
        model.fit(train_features, train_center)
        
        predicted_center = model.predict(valid_features)
        r2 = r2_score(valid_center, predicted_center, multioutput='uniform_average')
        mae = mean_absolute_error(valid_center, predicted_center, multioutput='uniform_average')
        
        r2_scores.append(r2)
        mae_scores.append(mae)

    avg_r2 = np.mean(r2_scores)
    avg_mae = np.mean(mae_scores)

    print(f"Sredni R-Squared: {avg_r2:.3f}")
    print(f"Sredni MAE: {avg_mae:.3f}")

    final_model = MultiOutputRegressor(lgb.LGBMRegressor(**best_params))
    final_model.fit(features, center)
    
    return final_model

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Blad: Brak sciezki do pliku JSON lub sciezki do zapisu modelu.")
        sys.exit(1)

    json_file_path = sys.argv[1]
    model_output_path = sys.argv[2]

    try:
        with open(json_file_path, 'r') as file:
            data = json.load(file)

        model = train_lightgbm_multioutput(data)
        print("Model wytrenowany pomyslnie.")

        joblib.dump(model, model_output_path)
        print(f"Model zapisany pomyslnie pod sciezka: {model_output_path}")

    except json.JSONDecodeError as e:
        print("Blad w dekodowaniu JSON:", str(e))
        sys.exit(1)
    except FileNotFoundError:
        print("Plik JSON nie zostal znaleziony:", json_file_path)
        sys.exit(1)
    except Exception as e:
        print("Blad w skrypcie Pythona:", str(e))
        sys.exit(1)
