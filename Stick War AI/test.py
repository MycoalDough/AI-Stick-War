import torch as T

print(T.device("cuda" if T.cuda.is_available() else "cpu"))