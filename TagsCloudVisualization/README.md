Алгоритм довольно простой:
1) Начиная с нулевого угла, вычисляем координаты точки на спирали в полярной системе координат по формуле `r(t) = t`
2) Переводим декартову систему координат
3) Создаем прямоугольник, проверяем его на пересечение с другими
4) Если он пересекается, то делаем сдвиг на определенный угол и переходим к пункту 1.

Угол определяется следующим образом: 

`angle += BaseDeltaAngle * (StretchYCoefficient / (iteration * stretchXCoefficient + 1) + LimitOfMultipliers)`

`BaseDeltaAngle` = PI/15

`StretchYCoefficient` = 3/4

`LimitOfMultipliers` = 1/2

`stretchXCoefficient` обратно пропорцианален площади прямоугольника.

`iteration` лежит в `[0; int.Max - 1]`

Идея заключается в том, что чем ближе к центру, тем плотнее располагаются прямоугольники.
Из этого вытекают 2 вещи:
1) Чем больше прямоугольник, тем больше шаги
2) Чем дальше от центра, тем мельче шаги
Данные функции балансируют друг друга таким образом: в начале размер шага разный для всех прямоугольников, но при стремлении i в бесконечность, он стремится к PI/30


**Война и мир, 50 слов длиннее трех букв, размер шрифта от 10 до 80**
![1](https://raw.githubusercontent.com/Yewert/tdd/development/TagsCloudVisualization/TagsCloudVisualization/coolWords1.png)

**Война и мир, 100 слов длиннее трех букв, размер шрифта от 20 до 120**
![2](https://raw.githubusercontent.com/Yewert/tdd/development/TagsCloudVisualization/TagsCloudVisualization/coolWords2.png)

**Война и мир, 500 слов длиннее трех букв, размер шрифта 0т 20 до 120**
![3](https://raw.githubusercontent.com/Yewert/tdd/development/TagsCloudVisualization/TagsCloudVisualization/coolWords3.png)

**Анджей Сапковски "Башня ласточки, 500 слов длиннее трех букв, размер шрифта 0т 10 до 120**
![4](https://raw.githubusercontent.com/Yewert/tdd/development/TagsCloudVisualization/TagsCloudVisualization/witcher.png)

_Синие прямоугольники - те размеры, по которым генерировалось облако. Они чуть больше, чем само слово._
