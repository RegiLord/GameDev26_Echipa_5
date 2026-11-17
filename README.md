# Infinite Fantasy

Un city builder / tower defense 3D. Ideea jocului este să aperi core-ul tău, balansând resource gathering cu defenses. Ca defense-uri
o să ai turnuri, ziduri, trap-uri + eroi și units pe care le poți coordona 

# Prototip (Stagiu actual)

Stagiul actual al prototipuli se află pe branch-uri separate, pe care dezvoltăm feature-uri separate

1. Unit_Towers
    
    - Aici dezvoltăm scripturile pentru unit-uri și towers
    
    - Momentan avem dezvoltate niște AI Controllere ( practic State Machines ) care controlează behaviourul uniturilor
    
    - Am introdus niște animații demo, cu ajutorul lui Mixamo ( auto-rigger + animations ). Modelul personajului este făcut de noi.
    
    - Unit-urile + Towerele sunt gândite pe module:
        - Health Module : Păstrează informații despre viață
        - Attack Module : Păstrează informații despre attack, attackSpeed, attackRange
        - MovementModule: Furnizează posibilitate să se miște

    - Ca exemplu un unit ar avea toate trei module-urile, dar un tower ar avea doar Health și Attack.

    - Toate aceste informații sunt gestionate de AIController la care avem ca componente
        - Blackboard (Inspirat din Unreal) : este un spațiu comun de date dinamice pe care toate stările îl pot accesa
        - AIStates : stările la automatul nostru care decid ce face la acel moment
            - FindEnemyState, FindClosestEnemyInDetectionRangeState, AttackState, MoveToState, DelayState
2. GUI
    
3. WaveManager

4. TowerPlacement
    
    - Este un branch mai vechi de la sprint-ul 0, nu conține încă ceva relevant, camera movementul prezent în branch va fi înlocuit de cel din GUI
    
    - Singurul lucru relevant pe acest branch este TowerPlacer script-ul care va fi mutat în proiectul principal
