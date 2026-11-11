import openpyxl
for f in [r"C:\Users\me\Desktop\xem3\conv\eProject_Status_Report(V1).xlsx",
          r"C:\Users\me\Desktop\xem3\conv\eProject_Feeback_Form.xlsx"]:
    print("=" * 20, f.split("\\")[-1], "=" * 20)
    wb = openpyxl.load_workbook(f, data_only=True)
    for ws in wb.worksheets:
        print(f"--- sheet: {ws.title} ({ws.max_row} rows x {ws.max_column} cols) ---")
        for row in ws.iter_rows(min_row=1, max_row=min(ws.max_row, 40)):
            vals = []
            for c in row:
                v = c.value
                if v is not None:
                    vals.append(f"{c.coordinate}={repr(v)}")
            if vals:
                print(" | ".join(vals))
