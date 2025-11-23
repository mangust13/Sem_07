(define (IsProperList? lst)
  (cond
    ((eq? (pair? lst) #f) #f)
    ((eq? (cdr lst) '()) #t)
    ((pair? (cdr lst)) (IsProperList? (cdr lst)))
    (#t #f)
    )
  )

(define (IsValidLength? lst)
  (cond
    ((eq? lst '()) #f)
    ((eq? (cdr lst) '()) #f)
    ((eq? (cddr lst) '()) #f)
    ((eq? (cdddr lst) '()) #f)
    ((eq? (cddddr lst) '()) #t)
    (#t #f)
  )
)

(define (IsValidLetters? lst)
  (cond
    ((eq? lst '()) #t)
    ((eq? (car lst) 'x) (IsValidLetters? (cdr lst)))
    ((eq? (car lst) 'y) (IsValidLetters? (cdr lst)))
    ((eq? (car lst) 'z) (IsValidLetters? (cdr lst)))
    (#t #f))
  )


(define (IsValidList? lst)
  (cond
    ((eq? (IsProperList? lst) #f) #f)
    ((eq? (IsValidLength? lst) #f) #f)
    ((eq? (IsValidLetters? lst) #f) #f)
    (#t #t))
  )

(define (ModifyList lst)
  (cond
    ((eq? (IsValidList? lst) #t)
      (cond
        ((eq? (caddr lst) (cadddr lst))
          (cons (car lst) (cons (cadr lst) '())))
        (#t
          (cons (car lst) (cadr lst)))))
      (#t 'Not-Valid-List)))
  

(ModifyList '(z y x x))
(ModifyList '(x y z x))

(ModifyList '(x y z a))
(ModifyList '(x y z x 2))
(ModifyList '(x . y))
(ModifyList 'x)
