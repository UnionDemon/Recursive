# Recursive
The last laboratory work.

<!DOCTYPE html>
<html>
<head>
	<title> Лабораторная работа 6 "Изучение и использование метода рекурсивного спуска для синтаксического анализа"</title>
<style>
        body {
            width: auto;
            height: auto;
            background-color: #cfb9b9;
            color: #000000;
            border: 2px solid #000000;
            margin: 1%;
        }
        h1 {
            text-align: center;
            letter-spacing: 5px;
        }
        h3, p {
            letter-spacing: 1px;
        }
	
	footer {
	box-sizing: border-box;
   	padding: 5px;
    	border-top: 3px solid black;
	}

    </style>
</head>

</head>
<body>
	<h1>Лабораторная работа 6 "Изучение и использование метода рекурсивного спуска для синтаксического анализа"</h1><hr>
	<h3 style="text-align: left">Содержание</h3>
	<ol>
		<li> <a href="#tt"><b>Техническое задание</b></a></li>
		<li> <a href="#grammar"><b>Грамматика</b></a></li>
		<li> <a href="#language"><b>Язык</b></a></li>
		<li> <a href="#classification"><b>Классификация</b></a></li>
		<li> <a href="#method"><b>Метод анализа - рекурсивный спуск</b></a></li>
		<li> <a href="#test"><b>Тестовые примеры</b></a></li>
	</ol><hr>
	<h3 id="tt">Техническое задание.</h3>
	<p>Цель работы: Изучить и использовать метод рекурсивного спуска для синтаксического анализа.</p>
	<p>Задание: Для грамматики G[E] разработать и реализовать алгоритм анализа на основе метода рекурсивного спуска.</p>
	<p>Пункт меню «Рекурсивный спуск» содержит подпункты: «Грамматика», «Язык», «Классификация», «Тестовый пример». Соответствующие пункты должны быть отражены в «Справке».</p>
	<p>В окне обработки текста отражается последовательность вызова процедур обработки символов грамматики в соответствии с деревом рекурсивного спуска.</p>
	<p>При наличии ошибки разбор продолжается с вышестоящего по отношению к ошибочному узла.</p><hr>
	<h3 id="grammar">Грамматика.</h3>
	<p>Определим грамматику арифметических выражений <b>G[E]</b> в нотации Хомского с продукциями P:</p>
	<p>E → TA</p>
	<p>A → ε | + TA | - TA</p>
	<p>T → ОВ</p>
	<p>В → ε | *ОВ | /ОВ</p>
	<p>О → num | id | (E)</p>
	<p>num – числовая константа Ц{Ц}</p>
	<p>id – идентификатор Б{Б|Ц}</p>
	<p>Б – [a, b, c, ...z, A, B, …, Z], Ц – [0, 1, …, 9]</p><br>
	<p>Следуя введенному формальному определению грамматики, представим G[E] ее составляющими:</p>
	<ul><li>Z = E;</li>
	<li>V<sub>T</sub> = {a, b, c, ..., z, A, B, C, ..., Z, +, -, *, /, (, ), 0, 1, 2, ..., 9};</li>
	<li>V<sub>N</sub> = {E, A, T, B, O}.</li></ul><hr>
	<h3 id="language">Язык.</h3>
	<p>Грамматика <b>G[E]</b> порождает язык арифметических выражений: </p>
	<p>L(G[E])={a+b–c, f*(b/(c+d)), 785+48*98, qr34/e2as+a21-wd14, ...}.</p>
	<p>Арифметическое выражение состоит из операторов и операндов. Операндами могут быть – числа или идентификаторы.</p>
	<p>Операторы обозначают выполняемые над ними действия (+ сложение, - вычитание, * умножение, / деление).</p><hr>
	<h3 id="classification">Классификация.</h3>
	<p>Согласно классификации Хомского, грамматика G[E] является контекстно-свободной, так как имеет вид:</p>
	<p>A &rarr; &alpha;, где A&isin;V<sub>N</sub>, &alpha;&isin;V<sup>*</sup>.</p><hr>
	<h3 id="method">Метод анализа - рекурсивный спуск.</h3>
	<p>Для грамматики G[E] был выбран метод рекурсивного спуска, потому что он подходит для контекстно-свободных грамматик.</p>
	<p>Метод рекурсивного спуска заключается в том, что для каждого нетерминала грамматики создается своя функция с именем этого нетерминала. Задача функции – начиная с указанного места исходной цепочки найти подцепочку, которая выводится из этого нетерминала. Тело каждой такой функции пишется по правилам вывода соответствующего нетерминала: терминалы из правой части распознаются самой функцией, а нетерминалы соответствуют вызовам функций. Функции могут вызывать сами себя.</p><hr>
	<h3 id="test">Тестовые примеры.</h3>
	<p>Тестирование разработанного сканера пресдтавлено на рисунках 1-5</p>
	<br><img src="images\example1.jpg" width="516px" height="240px" style=" margin-left: 33%; margin-right:33%">
	<figcaption style=" text-align: center">Рисунок 1 - Тестовый пример 1</figcaption><br>
	<br><img src="images\example2.jpg" width="516px" height="240px" style=" margin-left: 33%; margin-right:33%">
	<figcaption style=" text-align: center">Рисунок 2 - Тестовый пример 2</figcaption><br>
	<br><img src="images\example3.jpg" width="516px" height="240px" style=" margin-left: 33%; margin-right:33%">
	<figcaption style=" text-align: center">Рисунок 3 - Тестовый пример 3</figcaption><br>
	<br><img src="images\example4.jpg" width="516px" height="240px" style=" margin-left: 33%; margin-right:33%">
	<figcaption style=" text-align: center">Рисунок 4 - Тестовый пример 4</figcaption><br>
	<br><img src="images\example5.jpg" width="516px" height="240px" style=" margin-left: 33%; margin-right:33%">
	<figcaption style=" text-align: center">Рисунок 5 - Тестовый пример 5</figcaption><br>

	<footer><small>Лабораторная работа 6 выполнена бригадой №1 группы АВТ-812 (Антонянц Е., Амельченко А., Гостеева А.). Интерфейс разработан бригадой №1 группы АВТ-812 (Антонянц Е., Амельченко А., Гостеева А.)</small></footer>
</body>
</html>