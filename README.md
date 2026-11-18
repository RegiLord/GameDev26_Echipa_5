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
   - momentan doar un test pentru GUI / unit selection / camera
   - overhead camera cu unghiuri fixate
      - W/A/S/D: miscarea camerei
      - Q/E: rotatia la 90 de grade a camerei pe baza unui punct fix de pe pamant
      - Z/X: zoom in/out
   - unit selection
      - poti apasa click stanga pentru a selecta un unit sau left click and drag pentru a selecta mai multe unit-uri odata
      - "selectia" le schimba layer-ul pentru a le aplica un shader (nu da render bine din toate unghiurile la forme cu colturi ascutite, este o problema stiuta legata de metoda de implementare a outline-ului)
      - le pastreaza intr-o lista in SelectionManager, dar necesita ca obiectele care pot fi selectate sa aiba o componenta de tip SelectableUnit (script)
   - GUI
      - doar un prefab de test pentru resursele principale, va arata cat de cat similar cu ce e acum
      - creste la infinit si are doar milioanele definite pentru shortening dar va avea si valori mai mari (maybe si mai mici)
      - daca dai hover peste element, poti vedea niste stat-uri detaliate sub display (ar trebui sa aiba si un background, dar nu merge for now)
    
3. WaveManager

4. TowerPlacement
    
    - Este un branch mai vechi de la sprint-ul 0, nu conține încă ceva relevant, camera movementul prezent în branch va fi înlocuit de cel din GUI
    
    - Singurul lucru relevant pe acest branch este TowerPlacer script-ul care va fi mutat în proiectul principal


## Atlassian
[Link](https://infinitefantasy.atlassian.net/jira/software/projects/SCRUM/boards/1?filter=&groupBy=none&atlOrigin=eyJpIjoiYTA4YmY2YTliNjdhNDc1ZmExN2JkNTU0ZmI0Y2QzMTYiLCJwIjoiaiJ9) (S-ar putea sa nu fie accesabil)
<img src="images/jiro.png">

## Current Art

<img src="images/Swordsman.png">

![HeroSlash](./images/hero_slash.gif)