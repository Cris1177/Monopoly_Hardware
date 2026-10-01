extends Control

const SPACES: Array[Dictionary] = [
	{"name": "Salida", "kind": "start"},
	{"name": "Avenida Coto Brus", "kind": "property", "price": 60, "rent": 10, "group": 0},
	{"name": "CCSS", "kind": "ccss", "price": 50},
	{"name": "Avenida Belén Heredia", "kind": "property", "price": 60, "rent": 10, "group": 0},
	{"name": "Impuesto sobre la Renta", "kind": "tax", "price": 200},
	{"name": "Ferrocarril de Reading", "kind": "property", "price": 200, "rent": 25, "group": 7},
	{"name": "Avenida Agua Caliente", "kind": "property", "price": 100, "rent": 15, "group": 1},
	{"name": "Suerte", "kind": "event"},
	{"name": "Avenida Vermont", "kind": "property", "price": 100, "rent": 15, "group": 1},
	{"name": "Avenida Manuel de Jesús", "kind": "property", "price": 120, "rent": 20, "group": 1},
	{"name": "Cárcel / Solo de Visita", "kind": "jail"},
	{"name": "Plaza St. Charles", "kind": "property", "price": 140, "rent": 25, "group": 2},
	{"name": "Compañía ICE", "kind": "property", "price": 150, "rent": 30, "group": 8},
	{"name": "Avenida Aurora", "kind": "property", "price": 140, "rent": 25, "group": 2},
	{"name": "Avenida Purral", "kind": "property", "price": 160, "rent": 30, "group": 2},
	{"name": "Ferrocarril del Pacífico", "kind": "property", "price": 200, "rent": 35, "group": 7},
	{"name": "Plaza Guápiles de Limón", "kind": "property", "price": 180, "rent": 35, "group": 3},
	{"name": "CCSS", "kind": "ccss", "price": 50},
	{"name": "Avenida Alajuelita", "kind": "property", "price": 180, "rent": 35, "group": 3},
	{"name": "Avenida San Pedro", "kind": "property", "price": 200, "rent": 40, "group": 3},
	{"name": "Parque del TEC", "kind": "parking"},
	{"name": "Avenida Cartago City", "kind": "property", "price": 220, "rent": 45, "group": 4},
	{"name": "Suerte", "kind": "event"},
	{"name": "Avenida Desamparados", "kind": "property", "price": 220, "rent": 45, "group": 4}
]

const SERVER_IP := "127.0.0.1"  # IP de la compu que corre el servidor
const MY_ID := 1                # id de este jugador (1 a 4)

const PLAYER_NAMES := ["Christian", "Isaac", "Ulfran", "Fabricio"]
const PLAYER_COLORS := [Color("#e36b51"), Color("#55a6d8"), Color("#e5bd55"), Color("#8e76c5")]
const GROUP_COLORS := [
	Color("#9c755f"), Color("#75c8d4"), Color("#d785a4"), Color("#ef9558"),
	Color("#d6514a"), Color("#f0d15c"), Color("#70b878"), Color("#8e9199"),
	Color("#4d91b5"),
]

var players: Array[Dictionary] = []
var owners: Array[int] = []
var current_player := 0
var turn_number := 1
var has_rolled := false
var game_over := false
var tile_labels: Array[Dictionary] = []
var player_labels: Array[Label] = []
var turn_label: Label
var dice_label: Label
var detail_label: Label
var log_label: Label
var roll_button: Button
var buy_button: Button
var end_turn_button: Button
var net: Node  # nodo C# que habla con el servidor
var my_id := MY_ID            # se puede cambiar con --id=N al abrir el juego
var server_ip := SERVER_IP     # se puede cambiar con --ip=X.X.X.X


func _ready() -> void:
	for index in PLAYER_NAMES.size():
		players.append({"name": PLAYER_NAMES[index], "balance": 1500, "position": 0, "active": true})
	for _index in SPACES.size():
		owners.append(-1)
	_build_interface()
	_refresh_interface()
	
	# Argumentos al abrir el juego: --id=2 --ip=192.168.0.10
	for arg in OS.get_cmdline_user_args() + OS.get_cmdline_args():
		if arg.begins_with("--id="):
			my_id = int(arg.substr(5))
		if arg.begins_with("--ip="):
			server_ip = arg.substr(5)
	get_window().title = "Monopoly - Jugador %d" % my_id
	# Cliente de red: la pantalla solo pide acciones y dibuja lo que manda el servidor
	net = load("res://new-game-project/ClienteRedNode.cs").new()
	add_child(net)
	net.connect("EstadoRecibido", _on_estado)
	net.connect("MensajeServidor", _on_mensaje)
	if not net.Conectar(server_ip, my_id):
		log_label.text = "Sin conexión con el servidor en %s" % server_ip


func _build_interface() -> void:
	var background := ColorRect.new()
	background.color = Color("#142923")
	background.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	add_child(background)

	var margins := MarginContainer.new()
	margins.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	margins.add_theme_constant_override("margin_left", 24)
	margins.add_theme_constant_override("margin_top", 20)
	margins.add_theme_constant_override("margin_right", 24)
	margins.add_theme_constant_override("margin_bottom", 20)
	add_child(margins)

	var page := VBoxContainer.new()
	page.add_theme_constant_override("separation", 14)
	margins.add_child(page)

	var header := HBoxContainer.new()
	page.add_child(header)
	var title := _make_label("MONOPOLY  /  HARDWARE", 27, Color("#f3ead3"))
	header.add_child(title)
	var header_spacer := Control.new()
	header_spacer.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	header.add_child(header_spacer)
	var round_label := _make_label("PARTIDA LOCAL  ·  24 CASILLAS", 13, Color("#a6b7a8"))
	header.add_child(round_label)

	var content := HBoxContainer.new()
	content.size_flags_vertical = Control.SIZE_EXPAND_FILL
	content.add_theme_constant_override("separation", 22)
	page.add_child(content)

	var board_center := CenterContainer.new()
	board_center.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	board_center.size_flags_vertical = Control.SIZE_EXPAND_FILL
	content.add_child(board_center)
	_build_board(board_center)
	_build_sidebar(content)

	log_label = _make_label("Tu partida está lista.", 14, Color("#d5dfd3"))
	log_label.custom_minimum_size.y = 24
	page.add_child(log_label)


func _build_board(parent: Control) -> void:
	# Se indexan por número de casilla, aunque se creen en orden visual.
	tile_labels.resize(SPACES.size())
	var board := Control.new()
	board.custom_minimum_size = Vector2(620, 620)
	board.size_flags_horizontal = Control.SIZE_SHRINK_CENTER
	board.size_flags_vertical = Control.SIZE_SHRINK_CENTER
	parent.add_child(board)

	var frame := Panel.new()
	frame.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	frame.add_theme_stylebox_override("panel", _panel_style(Color("#24463a"), Color("#557563"), 14))
	board.add_child(frame)

	var grid := GridContainer.new()
	grid.columns = 7
	grid.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	grid.add_theme_constant_override("h_separation", 4)
	grid.add_theme_constant_override("v_separation", 4)
	grid.add_theme_constant_override("margin_left", 8)
	grid.add_theme_constant_override("margin_top", 8)
	grid.add_theme_constant_override("margin_right", 8)
	grid.add_theme_constant_override("margin_bottom", 8)
	board.add_child(grid)

	for row in 7:
		for column in 7:
			var space_index := _space_for_cell(row, column)
			if space_index < 0:
				var empty_cell := Control.new()
				empty_cell.custom_minimum_size = Vector2(72, 72)
				grid.add_child(empty_cell)
			else:
				grid.add_child(_make_tile(space_index))

	var center_panel := PanelContainer.new()
	center_panel.anchor_left = 1.0 / 7.0
	center_panel.anchor_top = 1.0 / 7.0
	center_panel.anchor_right = 6.0 / 7.0
	center_panel.anchor_bottom = 6.0 / 7.0
	center_panel.offset_left = 5
	center_panel.offset_top = 5
	center_panel.offset_right = -5
	center_panel.offset_bottom = -5
	center_panel.add_theme_stylebox_override("panel", _panel_style(Color("#1b352d"), Color("#557563"), 12))
	center_panel.mouse_filter = Control.MOUSE_FILTER_IGNORE
	board.add_child(center_panel)

	var center_content := VBoxContainer.new()
	center_content.alignment = BoxContainer.ALIGNMENT_CENTER
	center_content.add_theme_constant_override("separation", 12)
	center_panel.add_child(center_content)
	var mark := _make_label("M", 58, Color("#e5bd55"))
	mark.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	center_content.add_child(mark)
	var center_title := _make_label("EL TABLERO", 21, Color("#f3ead3"))
	center_title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	center_content.add_child(center_title)
	var center_note := _make_label("Propiedades · Suerte · Estrategia", 13, Color("#a6b7a8"))
	center_note.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	center_content.add_child(center_note)


func _build_sidebar(parent: Control) -> void:
	var sidebar := PanelContainer.new()
	sidebar.custom_minimum_size.x = 286
	sidebar.add_theme_stylebox_override("panel", _panel_style(Color("#203b32"), Color("#557563"), 12))
	parent.add_child(sidebar)

	var sections := VBoxContainer.new()
	sections.add_theme_constant_override("separation", 13)
	sidebar.add_child(sections)

	turn_label = _make_label("TURNO 1", 13, Color("#e5bd55"))
	sections.add_child(turn_label)
	detail_label = _make_label("", 17, Color("#f3ead3"))
	detail_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	sections.add_child(detail_label)

	var separator := HSeparator.new()
	sections.add_child(separator)
	sections.add_child(_make_label("JUGADORES", 12, Color("#a6b7a8")))
	for index in PLAYER_NAMES.size():
		var player_label := _make_label("", 14, Color("#f3ead3"))
		player_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
		sections.add_child(player_label)
		player_labels.append(player_label)

	var action_separator := HSeparator.new()
	sections.add_child(action_separator)
	dice_label = _make_label("DADOS   -  -", 18, Color("#f3ead3"))
	dice_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	sections.add_child(dice_label)

	roll_button = _make_button("Lanzar dados", Color("#c5a44d"), Color("#18271f"))
	roll_button.pressed.connect(_on_roll_pressed)
	sections.add_child(roll_button)
	buy_button = _make_button("Comprar propiedad", Color("#477a5a"), Color("#f3ead3"))
	buy_button.pressed.connect(_on_buy_pressed)
	sections.add_child(buy_button)
	end_turn_button = _make_button("Terminar turno", Color("#315647"), Color("#f3ead3"))
	end_turn_button.pressed.connect(_on_end_turn_pressed)
	sections.add_child(end_turn_button)

	var back_button := Button.new()
	back_button.text = "Volver al menú"
	back_button.flat = true
	back_button.add_theme_color_override("font_color", Color("#b5c3b5"))
	back_button.pressed.connect(func(): get_tree().change_scene_to_file("res://new-game-project/MainMenu.tscn"))
	sections.add_child(back_button)


func _make_tile(space_index: int) -> Control:
	var space: Dictionary = SPACES[space_index]
	var tile := PanelContainer.new()
	tile.custom_minimum_size = Vector2(72, 72)
	var tile_style := _panel_style(Color("#f0eadb"), Color("#d5cdbb"), 5)
	tile_style.content_margin_left = 3
	tile_style.content_margin_top = 3
	tile_style.content_margin_right = 3
	tile_style.content_margin_bottom = 3
	tile.add_theme_stylebox_override("panel", tile_style)

	var content := VBoxContainer.new()
	content.add_theme_constant_override("separation", 3)
	tile.add_child(content)

	var accent := ColorRect.new()
	accent.custom_minimum_size.y = 7
	accent.color = _space_color(space_index)
	content.add_child(accent)

	var name_label := _make_label(space["name"], 11, Color("#28352e"))
	name_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	name_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	name_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	name_label.size_flags_vertical = Control.SIZE_EXPAND_FILL
	content.add_child(name_label)

	var price_label := _make_label(_space_subtitle(space), 10, Color("#657066"))
	price_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	content.add_child(price_label)

	var owner_label := _make_label("", 9, Color("#28352e"))
	owner_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	content.add_child(owner_label)

	var tokens := HBoxContainer.new()
	tokens.alignment = BoxContainer.ALIGNMENT_CENTER
	tokens.custom_minimum_size.y = 10
	content.add_child(tokens)
	tile_labels[space_index] = {"name": name_label, "price": price_label, "owner": owner_label, "tokens": tokens}
	return tile


func _space_for_cell(row: int, column: int) -> int:
	# La casilla 0 está en la esquina inferior derecha; el recorrido avanza
	# hacia la izquierda por el borde inferior y continúa alrededor del tablero.
	if row == 6:
		return 6 - column
	if column == 0:
		return 12 - row
	if row == 0:
		return 12 + column
	if column == 6:
		return 18 + row
	return -1


func _space_color(space_index: int) -> Color:
	var space: Dictionary = SPACES[space_index]
	if space["kind"] != "property":
		match space["kind"]:
			"start": return Color("#d1aa4d")
			"event": return Color("#7c6aad")
			"tax": return Color("#c96c52")
			"ccss": return Color("#4f9b9b")
			"jail": return Color("#687984")
			_: return Color("#6b9c71")
	return GROUP_COLORS[space["group"]]


func _space_subtitle(space: Dictionary) -> String:
	if space["kind"] == "property":
		return "$%d  ·  renta $%d" % [space["price"], space["rent"]]
	if space["kind"] == "tax" or space["kind"] == "ccss":
		return "Paga $%d" % space["price"]
	return ""


# Botón "Tirar": genera los dos dados virtuales y delega todo en roll_with_value
func _on_roll_pressed() -> void:
	# El servidor decide el resultado: aquí solo se pide la acción
	net.Enviar("TIRAR_DADOS")


func roll_with_value(movement: int, dice_text: String = "") -> void:

	if has_rolled or game_over:
		return
	var player: Dictionary = players[current_player]
	if not player["active"]:
		return
	
	dice_label.text = dice_text if dice_text != "" else "DADO   %d" % movement
	var old_position: int = player["position"]
	player["position"] = (old_position + movement) % SPACES.size()
	if old_position + movement >= SPACES.size():
		player["balance"] += 200
		log_label.text = "%s pasó por Salida y recibió $200." % player["name"]
	else:
		log_label.text = "%s avanzó %d casillas." % [player["name"], movement]
	has_rolled = true
	_resolve_space(player)
	_refresh_interface()
	
func _on_dado_fisico(valor: int) -> void:
	roll_with_value(valor)


func _resolve_space(player: Dictionary) -> void:
	var space_index: int = player["position"]
	var space: Dictionary = SPACES[space_index]
	match space["kind"]:
		"property":
			var owner: int = owners[space_index]
			if owner == -1:
				log_label.text = "%s cayó en %s. Disponible por $%d." % [player["name"], space["name"], space["price"]]
			elif owner != current_player:
				var rent: int = space["rent"]
				if player["balance"] >= rent:
					player["balance"] -= rent
					players[owner]["balance"] += rent
					log_label.text = "%s pagó $%d de alquiler a %s." % [player["name"], rent, players[owner]["name"]]
				else:
					_eliminate_player(current_player)
					log_label.text = "%s no pudo pagar el alquiler y queda fuera." % player["name"]
			else:
				log_label.text = "%s volvió a su propiedad %s." % [player["name"], space["name"]]
		"tax":
			var tax: int = space["price"]
			if player["balance"] >= tax:
				player["balance"] -= tax
				log_label.text = "%s pagó $%d de impuestos." % [player["name"], tax]
			else:
				_eliminate_player(current_player)
				log_label.text = "%s no pudo pagar los impuestos y queda fuera." % player["name"]
		"ccss":
			var contribution: int = space["price"]
			if player["balance"] >= contribution:
				player["balance"] -= contribution
				log_label.text = "%s aportó $%d a la CCSS." % [player["name"], contribution]
			else:
				_eliminate_player(current_player)
				log_label.text = "%s no pudo aportar a la CCSS y queda fuera." % player["name"]
		"event":
			_apply_event(player)
		"start":
			log_label.text = "%s llegó a Salida." % player["name"]
		"jail":
			log_label.text = "%s está de visita en la cárcel." % player["name"]
		"parking":
			log_label.text = "%s descansó en el Parque del TEC." % player["name"]
	_check_game_over()


func _apply_event(player: Dictionary) -> void:
	var amount := 100
	if randi_range(0, 1) == 0:
		player["balance"] += amount
		log_label.text = "Suerte: %s recibió $%d del banco." % [player["name"], amount]
	elif player["balance"] >= amount:
		player["balance"] -= amount
		log_label.text = "Suerte: %s pagó $%d al banco." % [player["name"], amount]
	else:
		_eliminate_player(current_player)
		log_label.text = "%s no pudo pagar la carta y queda fuera." % player["name"]


func _on_buy_pressed() -> void:
	net.Enviar("COMPRAR_PROPIEDAD")


func _on_end_turn_pressed() -> void:
	net.Enviar("TERMINAR_TURNO")


func _eliminate_player(player_index: int) -> void:
	players[player_index]["active"] = false
	players[player_index]["balance"] = 0
	for space_index in owners.size():
		if owners[space_index] == player_index:
			owners[space_index] = -1


func _check_game_over() -> void:
	var active_count := 0
	var winner_name := ""
	for player in players:
		if player["active"]:
			active_count += 1
			winner_name = player["name"]
	if active_count <= 1:
		game_over = true
		if active_count == 1:
			log_label.text += "  %s gana la partida." % winner_name
		else:
			log_label.text = "La partida terminó sin ganador."


func _refresh_interface() -> void:
	var player: Dictionary = players[current_player]
	turn_label.text = "TURNO %d  ·  %s" % [turn_number, player["name"].to_upper()]
	var space: Dictionary = SPACES[player["position"]]
	detail_label.text = "En %s\nSaldo $%d" % [space["name"], player["balance"]]
	for index in players.size():
		var listed_player: Dictionary = players[index]
		var status := "ELIMINADO" if not listed_player["active"] else "$%d" % listed_player["balance"]
		player_labels[index].text = "%s  %s" % ["●", listed_player["name"] + "   " + status]
		player_labels[index].add_theme_color_override("font_color", PLAYER_COLORS[index] if listed_player["active"] else Color("#77857a"))
	for space_index in SPACES.size():
		var tile: Dictionary = tile_labels[space_index]
		var owner: int = owners[space_index]
		tile["owner"].text = "Dueño: " + players[owner]["name"] if owner >= 0 else ""
		for child in tile["tokens"].get_children():
			child.queue_free()
		for player_index in players.size():
			if players[player_index]["active"] and players[player_index]["position"] == space_index:
				var token := ColorRect.new()
				token.color = PLAYER_COLORS[player_index]
				token.custom_minimum_size = Vector2(9, 9)
				tile["tokens"].add_child(token)
	roll_button.disabled = has_rolled or game_over
	end_turn_button.disabled = not has_rolled or game_over
	var current_space: Dictionary = SPACES[player["position"]]
	buy_button.disabled = game_over or not has_rolled or current_space["kind"] != "property" or owners[player["position"]] != -1 or player["balance"] < current_space.get("price", 0)


func _make_label(text: String, size: int, color: Color) -> Label:
	var label := Label.new()
	label.text = text
	label.add_theme_font_size_override("font_size", size)
	label.add_theme_color_override("font_color", color)
	return label


func _make_button(text: String, color: Color, text_color: Color) -> Button:
	var button := Button.new()
	button.text = text
	button.custom_minimum_size.y = 46
	button.add_theme_font_size_override("font_size", 14)
	button.add_theme_color_override("font_color", text_color)
	button.add_theme_stylebox_override("normal", _panel_style(color, color, 7))
	button.add_theme_stylebox_override("hover", _panel_style(color.lightened(0.12), color.lightened(0.12), 7))
	button.add_theme_stylebox_override("pressed", _panel_style(color.darkened(0.12), color.darkened(0.12), 7))
	button.add_theme_stylebox_override("disabled", _panel_style(Color("#34483d"), Color("#34483d"), 7))
	return button


func _panel_style(background: Color, border: Color, radius: int) -> StyleBoxFlat:
	var style := StyleBoxFlat.new()
	style.bg_color = background
	style.border_color = border
	style.set_border_width_all(1)
	style.set_corner_radius_all(radius)
	style.content_margin_left = 10
	style.content_margin_top = 8
	style.content_margin_right = 10
	style.content_margin_bottom = 8
	return style


# Llega el estado oficial del servidor: la pantalla solo lo copia y lo dibuja
func _on_estado(json: String) -> void:
	var data: Variant = JSON.parse_string(json)
	if typeof(data) != TYPE_DICTIONARY:
		return
	for info in data["Jugadores"]:
		var index: int = int(info["Id"]) - 1
		if index < 0 or index >= players.size():
			continue
		players[index]["balance"] = int(info["Saldo"])
		# Módulo por si el tablero del servidor tuviera más casillas que la pantalla
		players[index]["position"] = int(info["Posicion"]) % SPACES.size()
		players[index]["active"] = bool(info["Activo"])
	# Dueños de propiedades: indice = casilla, valor = id del jugador (-1 = libre)
	var duenos: Array = data.get("Duenos", [])
	for space_index in owners.size():
		owners[space_index] = -1
	for space_index in duenos.size():
		if space_index < owners.size() and int(duenos[space_index]) > 0:
			owners[space_index] = int(duenos[space_index]) - 1
	if data["JugadorActualId"] != null:
		current_player = int(data["JugadorActualId"]) - 1
	turn_number = int(data["Turno"])
	has_rolled = bool(data.get("DadosLanzados", false))
	game_over = bool(data["JuegoTerminado"])
	_refresh_interface()
	dice_label.text = "DADOS   lanzados" if has_rolled else "DADOS   -  -"
	# Fin de partida: el servidor decide el ganador (mayor patrimonio = saldo + propiedades)
	if game_over:
		var ganador: Variant = data.get("Ganador", null)
		if ganador != null:
			log_label.text = "PARTIDA TERMINADA. Gana %s" % str(ganador)
		else:
			log_label.text = "PARTIDA TERMINADA."
		dice_label.text = "FIN DE LA PARTIDA"
		var resumen := "Patrimonio final\n"
		for info in data["Jugadores"]:
			resumen += "%s  $%d\n" % [info["Nombre"], int(info.get("Patrimonio", info["Saldo"]))]
		detail_label.text = resumen
	# Solo el jugador en turno puede usar los botones (el servidor también lo valida)
	var mi_turno := (current_player + 1) == my_id
	roll_button.disabled = roll_button.disabled or not mi_turno
	buy_button.disabled = buy_button.disabled or not mi_turno
	end_turn_button.disabled = end_turn_button.disabled or not mi_turno


# Respuestas y avisos del servidor (compras, errores, turnos...)
func _on_mensaje(accion: String, exito: bool, descripcion: String) -> void:
	if accion == "CONECTAR":
		log_label.text = "Conectado al servidor."
	else:
		log_label.text = descripcion
