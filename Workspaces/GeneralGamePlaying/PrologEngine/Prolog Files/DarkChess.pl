%%%:- module(dark_chess).
%%%:- export general_doomed/1.
%%%:- export surrounded/2.
%%%:- export can_be_captured/3.

% is it doomed for just this turn
general_doomed(P) :-
	role(P),
	piece(_, g, XG, YG, P, showing),
	
	% first see if the general is completely surrounded
	findall((XA, YA), (adjacent(XG, YG, XA, YA), piece(_, PieceType, XA, YA, _, _)), AdjacentOccupiedSquareList),
	length(AdjacentOccupiedSquareList, NumAdjacentOccupiedSquares),
	findall((XA, YA), adjacent(XG, YG, XA, YA), AdjacentSquareList),
	% by trying to unify NumAdjacentOccupiedSquares from the previous length
	% statement to this one we're really just seeing if all the adjacent squares
	% are occupied
	length(AdjacentSquareList, NumAdjacentOccupiedSquares),
	
	% The general is surrounded so it can't move, now see if it can be captured 
	% by any adjacent pieces
	role(OtherPlayer),
	distinct(P, OtherPlayer),
	findall((XA, YA), (adjacent(XG, YG, XA, YA), canCaptureAdjacent(CapturingPieceType, g), piece(_, CapturingPieceType, XA, YA, OtherPlayer, showing)), AdjacentCapturePieceLocations),
	length(AdjacentCapturePieceLocations, NumAdjacentCapturePieceLocations),
	NumAdjacentCapturePieceLocations =\= 0.
	
surrounded(X, Y) :-		
	index_x(X),
	index_y(Y),
	findall((XA, YA), (adjacent(X, Y, XA, YA), piece(_, _, XA, YA, _, _)), AdjacentOccupiedSquareList),
	length(AdjacentOccupiedSquareList, NumAdjacentOccupiedSquares),
	findall((XA, YA), adjacent(X, Y, XA, YA), AdjacentSquareList),
	% by trying to unify NumAdjacentOccupiedSquares from the previous length
	% statement to this one we're really just seeing if all the adjacent squares
	% are occupied
	length(AdjacentSquareList, NumAdjacentOccupiedSquares).
	
can_be_captured(X, Y, NumCapturingLocations) :-
	piece(_, PieceType, X, Y, Player, showing),	
	role(OtherPlayer),
	distinct(Player, OtherPlayer),
	findall((XA, YA), (adjacent(X, Y, XA, YA), canCaptureAdjacent(CapturingPieceType, PieceType), piece(_, CapturingPieceType, XA, YA, OtherPlayer, showing)), AdjacentCapturingPieceLocations),
	length(AdjacentCapturingPieceLocations, NumAdjacentCapturingPieceLocations),
	findall((XS, YS), (screened(X, Y, XS, YS), piece(_, c, XS, YS, OtherPlayer, showing)), CapturingCannonLocations),
	length(CapturingCannonLocations, NumCapturingCannonLocations),
	(NumAdjacentCapturingPieceLocations =\= 0; NumCapturingCannonLocations =\= 0),
	NumCapturingLocations is NumAdjacentCapturingPieceLocations + NumCapturingCannonLocations.  
	
can_be_captured(X, Y, AdjacentCapturingPieceLocations, CapturingCannonLocations) :-
	piece(_, PieceType, X, Y, Player, showing),	
	role(OtherPlayer),
	distinct(Player, OtherPlayer),
	findall((CapturingPieceType, XA, YA), (adjacent(X, Y, XA, YA), canCaptureAdjacent(CapturingPieceType, PieceType), piece(_, CapturingPieceType, XA, YA, OtherPlayer, showing)), AdjacentCapturingPieceLocations),
	length(AdjacentCapturingPieceLocations, NumAdjacentCapturingPieceLocations),
	findall((c, XS, YS), (screened(X, Y, XS, YS), piece(_, c, XS, YS, OtherPlayer, showing)), CapturingCannonLocations),
	length(CapturingCannonLocations, NumCapturingCannonLocations),
	(NumAdjacentCapturingPieceLocations =\= 0; NumCapturingCannonLocations =\= 0).  
	
piece_count(Player, HiddenLocations, NumHiddenLocations, ShowingLocations, NumShowingLocations, TotalCount) :-
	role(Player),
	findall((X, Y), piece(_, _, X, Y, Player, showing), ShowingLocations),		
	findall((X, Y), piece(_, _, X, Y, Player, hidden), HiddenLocations),
	length(ShowingLocations, NumShowingLocations),
	length(HiddenLocations, NumHiddenLocations),
	TotalCount is NumShowingLocations + NumHiddenLocations.

piece_count(NumPieces) :-
	findall(X, piece(_, _, X, _, _, _), Locations),
	length(Locations, NumPieces).

% n is total, nr is revealed, nf is hidden
piece_count(NumHiddenPieces, NumShowingPieces, TotalNumPieces) :-
	findall(X, piece(_, _, X, _, _, hidden), HiddenLocations),
	length(HiddenLocations, NumHiddenPieces),
	findall(X, piece(_, _, X, _, _, showing), ShowingLocations),
	length(ShowingLocations, NumShowingPieces),
	TotalNumPieces is NumHiddenPieces + NumShowingPieces. 

hidden_count(NumHiddenPieces) :-
	findall(X, piece(_, _, X, _, _, hidden), HiddenLocations),
	length(HiddenLocations, NumHiddenPieces).

showing_count(NumShowingPieces) :-
	findall(X, piece(_, _, X, _, _, showing), ShowingLocations),
	length(ShowingLocations, NumShowingPieces).

piece_types_hidden(Count) :-
	setof((Player, PieceType), piece(_, PieceType, _, _, Player, hidden), HiddenPieceTypes),
	length(HiddenPieceTypes, Count).
	
piece_types_in_state(Count, State) :-
	pieceState(State),
	setof((Player, PieceType), piece(_, PieceType, _, _, Player, State), PieceTypesInState),
	length(PieceTypesInState, Count).

piece_type_fully_revealed(Player, PieceType) :-
	role(Player),
	pieceType(PieceType),
	findall(PieceType, piece(_, PieceType, _, _, Player, hidden), HiddenPieceTypeList),
	length(HiddenPieceTypeList, 0).
	

% Procedure: Minimax AlphaBeta
% Input: Position u, value a, value b
% Output: Value at root 	

minimaxAlphaBeta(State, Value, MaxDepth) :-
	minimaxAlphaBeta(State, Value, MaxDepth, true).
	
minimaxAlphaBeta(State, Value, MaxDepth, true) :-
	MaxDepth > 0,
	%legal(State, _, Player, Move),
	%next(State, Move, 
	NextMaxDepth is MaxDepth - 1,
	minimaxAlphaBeta(State, Value, NextMaxDepth, false). 
	
minimaxAlphaBeta(State, Value, MaxDepth, false) :-
	MaxDepth > 0,
	NextMaxDepth is MaxDepth - 1,
	minimaxAlphaBeta(State, Value, NextMaxDepth, false). 
	
minimaxAlphaBeta(State, Value, 0, _) :-
	eval(State, Value).

all_legals_sorted(Player, SortedMoves) :- all_legals(Player, Moves), quicksort(Moves, SortedMoves).

gt(capture(_, _, _, _), move(_, _, _, _)) :- !.
gt(capture(_, _, _, _), flip(_, _)) :- !.
gt(flip(_, _), move(_, _, _, _)) :- !. 	
gt(_, noop) :- !.

quicksort(List, Sorted) :-
	quicksort2(List, Sorted - []).
	
quicksort2([], Z - Z).
quicksort2([X | Tail], A1 - Z2) :-
	split(X, Tail, Small, Big),
	quicksort2(Small, A1 - [X | A2]),
	quicksort2(Big, A2 - Z2).

split(X, [], [], []).
split(X, [Y | Tail], [Y | Small], Big) :-
	gt(X, Y), !,
	split(X, Tail, Small, Big).
split(X, [Y | Tail], Small, [Y | Big]) :-
	split(X, Tail, Small, Big).	

% eval_pmove	
eval_pmove(PMove, Value) :-
	PMove =.. [_, _, Move],
	Move =.. [capture, _, _, X2, Y2],
	!,
	piece(_, CapturedPieceType, X2, Y2, OtherPlayer, _),
	piece_val(OtherPlayer, CapturedPieceType, _, _, CapturedPieceValue),
	Value is CapturedPieceValue + 3.
	
eval_pmove(PMove, 2) :- 
	PMove =.. [_, _, Move],
	Move =.. [flip | _],
	!.
	
eval_pmove(PMove, 1) :- 
	PMove =.. [_, _, Move],
	Move =.. [move | _],
	!.
	 
% eval_move	 
eval_move(Player, Move, 0) :- 
	Move =.. [noop | _],
	!.

eval_move(Player, Move, Value) :-
	Move =.. [capture, _, _, X2, Y2],
	!,
	piece(_, CapturedPieceType, X2, Y2, OtherPlayer, _),
	piece_val(OtherPlayer, CapturedPieceType, _, _, CapturedPieceValue),
	Value is CapturedPieceValue + 3.
	
eval_move(Player, Move, 2) :- 
	Move =.. [flip | _],
	!.
	
eval_move(Player, Move, 1) :- 
	Move =.. [move | _],
	!.
	 
eval_move(Player, Move, 0) :- 
	Move =.. [noop | _],
	!.
	
create_move_eval_list([], []).
	
create_move_eval_list([HeadPMove | RestOfPMoves], [NewEntry | EvalList]) :-
	create_move_eval_list(RestOfPMoves, EvalList),
	eval_move(HeadPMove, Value),
	NewEntry =.. [pmove_eval, HeadPMove, Value].
	
		
% Ideas:
%	vary values based on a piece's position (is it surrounded, protected, etc.)
% 	vary values based on game phase (like opening, mid and end game)
%	vary values based on number of pieces (not types of pieces) than can capture or be captured by that piece

% piece_val/4
piece_val(Player, PieceType, AttValue, DefValue, TotalValue) :-
	role(Player),
	pieceType(PieceType),
	piece_val_att(Player, PieceType, AttValue),
	piece_val_def(Player, PieceType, DefValue),
	TotalValue is AttValue + DefValue.
	
% piece_val_att/3	
piece_val_att(Player, PieceType, Value) :-
	pieceType(PieceType),
    distinct(PieceType, s),
    distinct(PieceType, c),    	
	role(Player),
	role(OtherPlayer),
	distinct(Player, OtherPlayer),	
	setof(OtherPieceType, C^OtherPieceType^X^Y^OtherPlayer^S^(canCaptureAdjacent(PieceType, OtherPieceType), piece(C, OtherPieceType, X, Y, OtherPlayer, S)), PieceTypesWeCanCaptureStillOnBoard),
	length(PieceTypesWeCanCaptureStillOnBoard, Value).
	
piece_val_att(Player, c, 7) :-
	role(Player).

piece_val_att(Player, s, 4) :-
	role(Player),
	role(OtherPlayer),
	distinct(Player, OtherPlayer),
	piece(_, g, _, _, OtherPlayer, _).	

piece_val_att(Player, s, Value) :-
	role(Player),
	role(OtherPlayer),
	distinct(Player, OtherPlayer),
	not(piece(_, g, _, _, OtherPlayer, _)),
	setof(OtherPieceType, C^OtherPieceType^X^Y^OtherPlayer^S^(canCaptureAdjacent(s, OtherPieceType), piece(C, OtherPieceType, X, Y, OtherPlayer, S)), PieceTypesWeCanCaptureStillOnBoard),
	length(PieceTypesWeCanCaptureStillOnBoard, Value).

% piece_val_def/3
piece_val_def(Player, PieceType, Value) :-
	pieceType(PieceType),
    	distinct(PieceType, c),    	
	role(Player),
	role(OtherPlayer),
	distinct(Player, OtherPlayer),
	findall(CapturingPieceType, canCaptureAdjacent(CapturingPieceType, PieceType), PartialCapturingPieceTypes),
	append(PartialCapturingPieceTypes, [c], CapturingPieceTypes), 	
	setof(CapturingPieceType, C^CapturingPieceType^X^Y^OtherPlayer^S^(member(CapturingPieceType, CapturingPieceTypes), piece(C, CapturingPieceType, X, Y, OtherPlayer, S)), PieceTypesThatCanCaptureUsStillOnBoard),
	length(PieceTypesThatCanCaptureUsStillOnBoard, TmpValue),
	Value is 7 - TmpValue.
	
piece_val_def(Player, c, 0) :-
	role(Player).




%:- end_module(dark_chess).