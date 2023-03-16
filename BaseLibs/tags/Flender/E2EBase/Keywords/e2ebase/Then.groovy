package e2ebase

import java.lang.annotation.Retention
import java.lang.annotation.RetentionPolicy

@Retention(RetentionPolicy.SOURCE)
public @interface Then {
	String value() default "";
}
