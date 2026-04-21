import i18n from "@/i18n";
import '@/extensions/string.extensions';
import {Error} from "./error";

export class SucceededOrNotResponse {
    errors: Error[] = [];
    succeeded: boolean;

    constructor(succeeded: boolean, errors?: Error[]) {
        this.succeeded = succeeded
        if (errors)
            this.errors = errors
    }
    
    getErrorMessages(translationLocation: string, fallbackKey?: string): string[] {
        const errorMessages: string[] = [];
        this.errors.forEach(error => {
            const errorKey = error.errorType.makeFirstLetterLowercase()
            const translationKey = `${translationLocation}.${errorKey}`
            const translatedMessage = i18n.t(translationKey)
            const hasTranslatedMessage = translatedMessage !== translationKey

            if (hasTranslatedMessage) {
                errorMessages.push(translatedMessage)
                return
            }

            if (error.errorMessage) {
                errorMessages.push(error.errorMessage)
                return
            }

            if (fallbackKey)
                errorMessages.push(i18n.t(fallbackKey))
            else
                errorMessages.push(i18n.t('validation.errorOccured'))
        })
        return errorMessages
    }
}
