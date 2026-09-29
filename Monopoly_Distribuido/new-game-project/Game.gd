extends Control

const BOARD = [
	{"name": "Salida", "type": "Salida", "price": 0},
	{"name": "Avenida Mediterráneo", "type": "Propiedad", "price": 60},
	{"name": "Caja de Comunidad", "type": "Comunidad", "price": 0},
	{"name": "Avenida Báltico", "type": "Propiedad", "price": 60},
	{"name": "Impuesto sobre la Renta", "type": "Impuesto", "price": 200},
	{"name": "Ferrocarril de Reading", "type": "Ferrocarril", "price": 200},
	{"name": "Avenida Oriental", "type": "Propiedad", "price": 100},
	{"name": "Suerte", "type": "Suerte", "price": 0},
	{"name": "Avenida Vermont", "type": "Propiedad", "price": 100},
	{"name": "Avenida Connecticut", "type": "Propiedad", "price": 120},
	{"name": "Cárcel / Solo de Visita", "type": "Carcel", "price": 0},
	{"name": "Plaza St. Charles", "type": "Propiedad", "price": 140},
	{"name": "Compañía Eléctrica", "type": "Servicio", "price": 150},
	{"name": "Avenida States", "type": "Propiedad", "price": 140},
	{"name": "Avenida Virginia", "type": "Propiedad", "price": 160},
	{"name": "Ferrocarril de Pennsylvania", "type": "Ferrocarril", "price": 200},
	{"name": "Plaza Guapiles de Limon", "type": "Propiedad", "price": 180},
	{"name": "Caja de Comunidad", "type": "Comunidad", "price": 0},
	{"name": "Avenida Alajuelita", "type": "Propiedad", "price": 180},
	{"name": "Avenida New York", "type": "Propiedad", "price": 200},
	{"name": "Parqueo Gratis", "type": "ParqueoGratis", "price": 0},
	{"name": "Avenida Cartago City", "type": "Propiedad", "price": 220},
	{"name": "Suerte", "type": "Suerte", "price": 0},
	{"name": "Avenida Desamparados", "type": "Propiedad", "price": 220},
	{"name": "Avenida Illinois", "type": "Propiedad", "price": 240},
	{"name": "Ferrocarril B&O", "type": "Ferrocarril", "price": 200},
	{"name": "Avenida Atlantic", "type": "Propiedad", "price": 260},
	{"name": "Avenida Ventnor", "type": "Propiedad", "price": 260},
	{"name": "Compañía de Agua", "type": "Servicio", "price": 150},
	{"name": "Jardines Marvin", "type": "Propiedad", "price": 280},
	{"name": "Ir a la Cárcel", "type": "IrCarcel", "price": 0},
	{"name": "Avenida Pacific", "type": "Propiedad", "price": 300},
	{"name": "Avenida Jaco", "type": "Propiedad", "price": 300},
	{"name": "Caja de Comunidad", "type": "Comunidad", "price": 0},
	{"name": "Avenida Pennsylvania", "type": "Propiedad", "price": 320},
	{"name": "Ferrocarril Short Line", "type": "Ferrocarril", "price": 200},
	{"name": "Suerte", "type": "Suerte", "price": 0},
	{"name": "Plaza Park", "type": "Propiedad", "price": 350},
	{"name": "Impuesto de Lujo", "type": "Impuesto", "price": 100},
	{"name": "Paseo Tablado", "type": "Propiedad", "price": 400}
]

const TYPE_ICONS = {
	"Salida": "res://addons/at-icons/node2d/arrow_up.svg",
	"Propiedad": "res://addons/at-icons/node2d/house.svg",
	"Comunidad": "res://addons/at-icons/node2d/speech_bubble_question.svg",
	"Impuesto": "res://addons/at-icons/node2d/traffic_lights.svg",
	"Ferrocarril": "res://addons/at-icons/node2d/arrow_right.svg",
	"Suerte": "res://addons/at-icons/node2d/speech_bubble_question.svg",
	"Carcel": "res://addons/at-icons/node2d/parking_sign.svg",
	"Servicio": "res://addons/at-icons/node2d/lightbulb.svg",
	"ParqueoGratis": "res://addons/at-icons/node2d/parking_sign.svg",
	"IrCarcel": "res://addons/at-icons/node2d/parking_sign.svg"
}

var players = []
var current_player_index = 0
var board_owned = {}
var board_labels = []

func _ready() -> void:
	initialize_players()
	build_board()
	refresh_players_panel()
	update_turn_ui()
	refresh_board()

func initialize_players() -> void:
	var colors = [Color(0.95, 0.2, 0.2), Color(0.2, 0.55, 0.95), Color(0.3, 0.85, 0.35), Color(0.95, 0.72, 0.16)]
	players = [
		{"name": "Cristian", "money": 1500, "position": 0, "in_jail": false, "jail_turns": 0, "color": colors[0]},
		{"name": "Isaac", "money": 1500, "position": 0, "in_jail": false, "jail_turns": 0, "color": colors[1]},
		{"name": "Ulfrán", "money": 1500, "position": 0, "in_jail": false, "jail_turns": 0, "color": colors[2]},
		{"name": "Fabricio", "money": 1500, "position": 0, "in_jail": false, "jail_turns": 0, "color": colors[3]}
	]
	board_owned.clear()
	current_player_index = 0

func build_board() -> void:
	var grid = $MainLayout/BoardPanel/BoardGrid
	for child in grid.get_children():
		child.queue_free()
	board_labels.clear()
	for i in range(BOARD.size()):
		var label = RichTextLabel.new()
		label.name = "Cell_%d" % i
		label.custom_minimum_size = Vector2(120, 70)
		label.fit_content = true
		label.bbcode_enabled = true
		label.scroll_active = false
		label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
		label.clip_contents = true
		label.modulate = Color(1, 1, 1, 1)
		grid.add_child(label)
		board_labels.append(label)

func refresh_board() -> void:
	for i in range(BOARD.size()):
		var cell = BOARD[i]
		var label = board_labels[i]
		var icon_path = TYPE_ICONS.get(cell["type"], "")
		var text = ""
		if i == 10:
			text = "Cárcel / Visita"
		elif cell["price"] > 0:
			text = "%s\n$%d" % [cell["name"], cell["price"]]
		else:
			text = cell["name"]
		if board_owned.has(i):
			text += "\nDueño: %s" % board_owned[i]
		if is_player_on_position(i):
			text += "\n★"
		if icon_path != "":
			text = "[img=18x18]%s[/img]\n%s" % [icon_path, text]
		label.text = text
		if is_player_on_position(i):
			label.modulate = Color(1, 1, 0.2, 1)
		else:
			label.modulate = Color(1, 1, 1, 1)

func refresh_players_panel() -> void:
	var list = $MainLayout/RightPanel/RightContent/PlayerList
	for child in list.get_children():
		child.queue_free()
	for i in range(players.size()):
		var player = players[i]
		var panel = PanelContainer.new()
		var box = VBoxContainer.new()
		var title = Label.new()
		var money = Label.new()
		var position = Label.new()
		var state = Label.new()
		title.text = player["name"] + (" (Turno)" if i == current_player_index else "")
		title.modulate = player["color"]
		title.add_theme_font_size_override("font_size", 16)
		money.text = "Dinero: $%d" % player["money"]
		position.text = "Posición: %d - %s" % [player["position"], BOARD[player["position"]]["name"]]
		state.text = "Estado: En la cárcel" if player["in_jail"] else "Estado: Libre"
		box.add_child(title)
		box.add_child(money)
		box.add_child(position)
		box.add_child(state)
		panel.add_child(box)
		list.add_child(panel)

func update_turn_ui() -> void:
	var player = players[current_player_index]
	$MainLayout/RightPanel/RightContent/TurnLabel.text = "Turno: %s" % player["name"]
	$MainLayout/RightPanel/RightContent/Actions/BuyButton.disabled = true
	$MainLayout/RightPanel/RightContent/Actions/EndTurnButton.disabled = false
	refresh_players_panel()

func is_player_on_position(index: int) -> bool:
	for player in players:
		if player["position"] == index:
			return true
	return false

func get_current_player() -> Dictionary:
	return players[current_player_index]

func get_player_by_name(name: String) -> Dictionary:
	for player in players:
		if player["name"] == name:
			return player
	return {}

func _on_roll_pressed() -> void:
	var player = get_current_player()
	var dice_1 = randi_range(1, 6)
	var dice_2 = randi_range(1, 6)
	var total = dice_1 + dice_2
	$MainLayout/RightPanel/RightContent/DiceLabel.text = "Dados: %d + %d = %d" % [dice_1, dice_2, total]

	if player["in_jail"]:
		if dice_1 == dice_2:
			player["in_jail"] = false
			player["jail_turns"] = 0
			player["position"] = (player["position"] + total) % BOARD.size()
			$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s sacó dobles y sale de la cárcel. Avanza a %s." % [player["name"], BOARD[player["position"]]["name"]]
			refresh_board()
			refresh_players_panel()
			return
		else:
			player["jail_turns"] += 1
			$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s está en la cárcel. Intento %d/2. No saca dobles." % [player["name"], player["jail_turns"]]
			if player["jail_turns"] >= 2:
				player["in_jail"] = false
				player["jail_turns"] = 0
				player["money"] -= 50
				$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s cumple dos turnos en la cárcel y paga $50 para salir." % player["name"]
			refresh_board()
			refresh_players_panel()
			return

	player["position"] = (player["position"] + total) % BOARD.size()
	var cell = BOARD[player["position"]]
	$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s avanzó a %s." % [player["name"], cell["name"]]

	if cell["type"] == "IrCarcel":
		player["position"] = 10
		player["in_jail"] = true
		player["jail_turns"] = 0
		$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s fue enviado a la cárcel. Casilla 10: Cárcel / Visita." % player["name"]
		refresh_board()
		refresh_players_panel()
		return

	if cell["type"] == "Propiedad" or cell["type"] == "Ferrocarril" or cell["type"] == "Servicio":
		if not board_owned.has(player["position"]):
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Está disponible por $%d." % cell["price"]
			$MainLayout/RightPanel/RightContent/Actions/BuyButton.disabled = false
		elif board_owned[player["position"]] == player["name"]:
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Ya tienes esta propiedad."
			$MainLayout/RightPanel/RightContent/Actions/BuyButton.disabled = true
		else:
			var rent = int(cell["price"] * 0.35)
			player["money"] -= rent
			var owner = get_player_by_name(board_owned[player["position"]])
			owner["money"] += rent
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Pagas alquiler de $%d a %s." % [rent, owner["name"]]
			$MainLayout/RightPanel/RightContent/Actions/BuyButton.disabled = true
	else:
		$MainLayout/RightPanel/RightContent/Actions/BuyButton.disabled = true
		if cell["type"] == "Impuesto":
			player["money"] -= cell["price"]
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Pagas impuesto de $%d." % cell["price"]
		elif cell["type"] == "Caja de Comunidad":
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Carta de comunidad: sigue jugando."
		elif cell["type"] == "Suerte":
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Carta de suerte: sigue adelante."
		elif cell["type"] == "Carcel":
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Estás en la casilla de cárcel / visita."
		elif cell["type"] == "ParqueoGratis":
			$MainLayout/RightPanel/RightContent/StatusLabel.text += " Estás en parqueo gratis."
	refresh_board()
	refresh_players_panel()

func _on_buy_pressed() -> void:
	var player = get_current_player()
	var position = player["position"]
	var cell = BOARD[position]
	if board_owned.has(position):
		$MainLayout/RightPanel/RightContent/StatusLabel.text = "Esta propiedad ya tiene dueño."
		return
	if player["money"] < cell["price"]:
		$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s no tiene dinero suficiente para comprar %s." % [player["name"], cell["name"]]
		return
	player["money"] -= cell["price"]
	board_owned[position] = player["name"]
	$MainLayout/RightPanel/RightContent/StatusLabel.text = "%s compró %s por $%d." % [player["name"], cell["name"], cell["price"]]
	$MainLayout/RightPanel/RightContent/Actions/BuyButton.disabled = true
	refresh_board()
	refresh_players_panel()

func _on_end_turn_pressed() -> void:
	current_player_index = (current_player_index + 1) % players.size()
	update_turn_ui()
	$MainLayout/RightPanel/RightContent/StatusLabel.text = "Es el turno de %s." % players[current_player_index]["name"]
	refresh_board()

func _on_back_pressed() -> void:
	get_tree().change_scene_to_file("res://MainMenu.tscn")
