% The alpha beta algorithm (from Bratko's 'Prolog Programming for Artificial Intelligence', 4th Edition)
/*
%  A game tree (Figure 22.2 translated to Prolog)

%  moves( Position, PositionList): possible moves

moves( a, [b,c]).
moves( b, [d,e]).
moves( c, [f,g]).
moves( d, [i,j]).
moves( e, [k,l]).
moves( f, [m,n]).
moves( g, [o,p]).

% min_to_move( Pos): Player 'min' to move in Pos

min_to_move( b).
min_to_move( c).

% max_to_move Pos): Player 'max' to move in Pos

max_to_move( a).
max_to_move( d).
max_to_move( e).
max_to_move( f).
max_to_move( g).

% staticval( Pos, Value):  Value is the static value of Pos

staticval( i, 1).
staticval( j, 4).
staticval( k, 5).
staticval( l, 6).
staticval( m, 2).
staticval( n, 1).
staticval( o, 1).
staticval( p, 1).
*/
/*
alphabeta(Pos, Alpha, Beta, GoodPos, Val) :-
	moves(Pos, PosList), !,
	boundedbest(PosList, Alpha, Beta, GoodPos, Val);
	staticval(Pos, Val).		% Static value of Pos
	
boundedbest([Pos | PosList], Alpha, Beta, GoodPos, GoodVal) :-
	alphabeta(Pos, Alpha, Beta, _, Val),
	goodenough(PosList, Alpha, Beta, Pos, Val, GoodPos, GoodVal).
	
goodenough([], _, _, Pos, Val, Pos, Val) :- !.	% no other candidate

goodenough(_, Alpha, Beta, Pos, Val, Pos, Val) :-
	min_to_move(Pos), Val > Beta, !;			% Maximizer attained upper bound
	max_to_move(Pos), Val < Alpha, !.			% Minimizer attained lower bound
	
goodenough(PosList, Alpha, Beta, Pos, Val, GoodPos, GoodVal) :-
	newbounds(Alpha, Beta, Pos, Val, NewAlpha, NewBeta), 	% redefine bounds
	boundedbest(PosList, NewAlpha, NewBeta, Pos1, Val1),
	betterof(Pos, Val, Pos1, Val1, GoodPos, GoodVal).
	
newbounds(Alpha, Beta, Pos, Val, Val, Beta) :- 
	min_to_move(Pos), Val > Alpha, !.			% Maximizer increased lower bound
	
newbounds(Alpha, Beta, Pos, Val, Alpha, Val) :-
	max_to_move(Pos), Val < Beta, !.			% Minimizer decreased upper bound
	
newbounds(Alpha, Beta, _, _, Alpha, Beta).		% Otherwise bounds unchanged

betteroff(Pos, Val, Pos1, Val1, Pos, Val) :-	% Pos better than Pos1
	min_to_move(Pos), Val > Val1, !;
	max_to_move(Pos), Val < Val1, !.
	
betterof(_, _, Pos1, Val1, Pos1, Val1).			% Otherwise Pos1 better
*/		

% end of Bratko's original alpha beta algorithm

% start of alpha beta algorithm from 'The Art of Prolog'

% evaluate_and_choose(Moves, Position, Depth, Alpha, Beta, Record, BestMove)
%	Chooses the BestMove from the set of Moves from the current Position using
%	the minimax algorithm with the alpha-beta cutoff searching Depth ply ahead.
%	Alpha and Beta are the parameters of the algorithm.  Record records the 
%	current best move.

evaluate_and_choose([Move | Moves], Position, Depth, Alpha, Beta, Record, BestMove) :-
	move(Move, Position, Position1),
	alpha_beta(Depth, Position1, Alpha, Beta, _MoveX, Value),
	Value1 is -Value,
	cutoff(Move, Value1, Depth, Alpha, Beta, Moves, Position, Record, BestMove).
evaluate_and_choose([], _Position, _Depth, Alpha, _Beta, Move, (Move, Alpha)).

alpha_beta(0, Position, _Alpha, _Beta, _NoMove, Value) :-
	value(Position, Value).	
alpha_beta(Depth, Position, Alpha, Beta, Move, Value) :-
	Depth > 0,
	findall(M, move(Position, M), Moves),
	Alpha1 is -Beta,
	Beta1 is -Alpha,
	Depth1 is Depth-1,
	evaluate_and_choose(Moves, Position, Depth1, Alpha1, Beta1, [], (Move, Value)).
	
cutoff(Move, Value, _Depth, _Alpha, Beta, _Moves, _Position, _Record, (Move, Value)) :-
	Value >= Beta.
cutoff(Move, Value, Depth, Alpha, Beta, Moves, Position, _Record, BestMove) :-
	Alpha < Value, Value < Beta,
	evaluate_and_choose(Moves, Position, Depth, Value, Beta, Move, BestMove).
cutoff(_Move, Value, Depth, Alpha, Beta, Moves, Position, Record, BestMove) :-
	Value =< Alpha,
	evaluate_and_choose(Moves, Position, Depth, Alpha, Beta, Record, BestMove). 
	
% end of alpha beta algorithm from 'The Art of Prolog' 
 