import json
res = []

with open('changes.json') as f:
    dataLis = json.load(f)
    for data in dataLis:
        tmpJson = {
            "FileName": data["FileName"],
            "FilePath": data["FilePath"],
            "LineNumber": data["FileChanges"][0]["LineNumber"],
            "CharNumber": data["FileChanges"][0]["CharNumber"],
            "DiagnosticId": data["FileChanges"][0]["DiagnosticId"],
            "FormatDescription": data["FileChanges"][0]["FormatDescription"],
        }
        res.append(tmpJson)

with open('changesToBeMade.json', 'w') as f:
    json.dump(res, f)
