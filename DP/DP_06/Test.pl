
father_with_2plus_children(Father):-
    family(Father, _, [_, _ | _]).

mother_with_children(Mother):-
    family(_, Mother, [_ | _]).

family_3+(F, M):-
    family(F, M, [ _, _, _, | _]).

father_first_child_unemployed(Father):-
    family(F, _, [person(_, _, _, unemployed) | _]).

child_of_father(Father, Children) :-
    family(Father, _, Children),