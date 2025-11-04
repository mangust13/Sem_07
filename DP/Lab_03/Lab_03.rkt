(define (ВІДОБРАЗИТИ lst func)
  (cond
    ((eq? lst '()) '())
    (#t (cons (func (car lst)) (ВІДОБРАЗИТИ (cdr lst) func)))))

(define (ОБЕРНЕНИЙ lst)
  (define (helper sublist acc)
    (cond
      ((eq? sublist '()) acc)
      (#t (helper (cdr sublist) (cons (car sublist) acc)))))
  (helper lst '()))

(define (ОБЕРНЕНИЙ_ДЕКРЕМЕНТ x)
  (cond
    ((eq? x '()) '())
    ((pair? x) (ОБЕРНЕНИЙ (ВІДОБРАЗИТИ x ОБЕРНЕНИЙ_ДЕКРЕМЕНТ)))
    (#t (- x 1))))


(ОБЕРНЕНИЙ_ДЕКРЕМЕНТ '(2 6 (5 4) 3 ((8 9) 0)))
