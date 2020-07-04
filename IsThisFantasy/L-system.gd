extends Node

# variables: A B
# axiom: A
# rules: (A → AB), (B → A)
var angle;
var axiom = "A";
var sentence = axiom;
var length = 100;
var rules = []



# Called when the node enters the scene tree for the first time.
func _ready():
	appendRule("A", "A-[[B]+B]+A[+AB]-B")
	generate()
	pass # Replace with function body.

func appendRule(A, B):
	var rule = {
		a = A,
		b = B
	}
	rules.append(rule)

func generate():
	length *= 0.5;
	var nextSentence = "";
	for i in range (sentence.length()):
		var current = sentence[i];
		var found = false;
		for j in range (rules.size()):
			if (current == rules[j].a):
				found = true;
				nextSentence += rules[j].b;
				break;
				
		if (!found):
			nextSentence += current;
	sentence = nextSentence;
	
	print(sentence)

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
