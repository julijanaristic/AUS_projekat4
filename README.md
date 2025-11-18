# Ice Cream Mixer – SCADA Simulation Project

## Description
A SCADA-based simulation system for automated ice cream production, featuring valve control, mixer management, Modbus communication, alarm supervision, and process visualization.  
This project simulates the complete workflow of filling, mixing, and emptying a mixer based on strict industrial automation rules.

## Authors
- [Katarina Kalauz](https://github.com/kalauzkatarina)
- [Julijana Ristić](https://github.com/julijanaristic)

---

## System Overview

### Components
- **Valve V1 – Chocolate** (50 kg/sec)  
- **Valve V2 – Milk** (50 L/sec)  
- **Valve V3 – Water** (30 L/sec)  
- **Valve V4 – Discharge**  
- **Mixer Motor**  
- **Analog Output – Ingredient Quantity (kg)**  

Only **one valve can be open at a time**.  
The mixer **cannot run during filling**.

---

## Process Workflow

1. Start the ice-cream production process (`Start = 1`).
2. Fill the mixer in the following order:
   - 100 kg of chocolate via **V1**
   - 150 L of milk via **V2**
   - 120 L of water via **V3**
3. After filling, start the mixer and run it for **10 seconds**.
4. Empty the mixer via **V4** at **100 kg/sec**.
5. Reset `Start = 0` and allow the process to restart.

### Safety Logic
If any filling valve (V1, V2, V3) opens during mixing:
- Stop the mixer  
- Close all valves V1, V2, V3  
- Immediately empty the mixer via V4  

---

## Modbus Configuration

### RTU Settings
- **Slave Address:** 55  
- **Protocol:** TCP  
- **Port:** 503  

### Digital Outputs (Coils)
Each coil includes:
- Default value: `0`
- `AbnormalValue`:  
  - Valves → abnormal when **open**
  - Mixer motor → abnormal when **ON**

| Coil | Address | Description |
|------|---------|-------------|
| Start | 3000 | Process start indication |
| Mixer Motor | 3001 | ON/OFF state |
| Valve V1 | 4000 | Chocolate valve |
| Valve V2 | 4001 | Milk valve |
| Valve V3 | 4002 | Water valve |
| Valve V4 | 4003 | Discharge valve |

---

## Analog Outputs (Holding Registers)

### Parameters
- **A:** 1 (scaling factor)  
- **B:** 0 (offset)  
- **HighAlarm:** 350 kg  
- **LowAlarm:** 50 kg  

### Conversion Formula
```bash
EGU_value = A * raw_value + B
```

### Alarm Conditions
- Quantity > HighAlarm → **High Alarm**
- Quantity < LowAlarm → **Low Alarm**

---

## User Interface Requirements

### Must include:
- Periodic reading of all coils and registers  
- Alarm visualization  
- Manual control over:
  - All valves
  - Mixer motor
  - Start signal  
- Ability to write analog values (converted from engineering units to raw)

---

## Simulation Logic

### Filling Phase
The UI must simulate the flow:
- **Chocolate:** +50 kg/sec  
- **Milk:** +50 L/sec  
- **Water:** +30 L/sec  

### Mixing Phase
- Run the mixer for **10 seconds**  
- Mixer must be OFF during filling  

### Emptying Phase
- Discharge rate: **100 kg/sec**  

---

## Repeatability
The entire automated cycle can be executed unlimited times.

---

## Emergency Conditions
Triggered if a filling valve opens during mixing:
- Mixer OFF  
- Close V1, V2, V3  
- Empty mixer  
- Update alarms  

---

## Summary
This SCADA project simulates a complete industrial mixing process with:
- Realistic valve/flow modeling  
- Alarm handling  
- Modbus TCP communication  
- Process automation and visualization  
- Configurable analog/digital points via **RtuCfg.txt**  
