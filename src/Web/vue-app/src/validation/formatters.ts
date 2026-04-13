export function formatPhoneNumberInput(value: string): string {
  const digitsOnly = value.replace(/\D/g, "").slice(0, 10);

  if (digitsOnly.length <= 3) {
    return digitsOnly;
  }

  if (digitsOnly.length <= 6) {
    return `${digitsOnly.slice(0, 3)}-${digitsOnly.slice(3)}`;
  }

  return `${digitsOnly.slice(0, 3)}-${digitsOnly.slice(3, 6)}-${digitsOnly.slice(6, 10)}`;
}

export function formatPostalCodeInput(value: string): string {
  const normalizedValue = value.toUpperCase();
  let compactValue = "";

  for (const character of normalizedValue) {
    if (!/[A-Z0-9]/.test(character)) {
      continue;
    }

    const position = compactValue.length;
    if (position >= 6) {
      break;
    }

    const mustBeLetter = position % 2 === 0;
    if (mustBeLetter && /[A-Z]/.test(character)) {
      compactValue += character;
      continue;
    }

    if (!mustBeLetter && /\d/.test(character)) {
      compactValue += character;
    }
  }

  if (compactValue.length <= 3) {
    return compactValue;
  }

  return `${compactValue.slice(0, 3)} ${compactValue.slice(3, 6)}`;
}
